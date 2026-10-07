using UnityEngine;
using Unity.MLAgents;
using System;
using Unity.Mathematics;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ML_BirdAgent : Agent
{
    [Tooltip("이동력")]
    public float moveForce = 2f;
    [Tooltip("고개를 기울이는 속도")]
    public float pitchSpeed = 100f;
    [Tooltip("회전 속도")]
    public float yawSpeed = 100f;
    [Tooltip("부리 끝 부분의 위치")]
    public Transform beakTip;
    [Tooltip("에이전트 카메라")]
    public Camera agentCamera;
    [Tooltip("학습 모드")]
    public bool trainingMode;

    private Rigidbody agentRb;
    //화단
    private ML_FlowerArea flowerArea;
    // 가장 가까운 꽃
    private ML_Flower nearestFlower;
    //부드러운 높낮이 변화를 위한 가중치
    private float smoothPitchChange = 0f;
    private float smoothYawChange = 0f;
    //새의 위아래 회전 각도 제한 -> 제한하지 않으면 새가 뒤집혀서 날아가는 경우가 생김
    private const float maxPitchAngle = 80f;
    // 넥타르 콜리전과 닿는 부리 끝 부분으로부터의 거리
    private const float beatTipRadius = .008f;
    //에이전트가 현재 멈춰있는지 판단하는 변수
    private bool frozen = false;

    /// <summary>
    /// 현재 에피소드에서 에이전트가 획득한 넥타르의 양
    /// </summary>
    public float NectarObtained { get; private set; }

    // 에이전트 초기화
    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
        flowerArea = GetComponentInParent<ML_FlowerArea>();

        //훈련모드가 아니라면 단계없음, 무제한 플레이
        if (!trainingMode) MaxStep = 0;
    }

    /// <summary>
    /// 에피소드 시작 시 에이전트 리셋
    /// </summary>
    public override void OnEpisodeBegin()
    {
        if (trainingMode)
        {
            // 훈련 모드일 경우 화단의 꽃을 재설정
            flowerArea.ResetFlower();
        }
        //넥타르 획득량 초기화
        NectarObtained = 0f;

        // 에이전트의 위치와 각도를 초기화
        agentRb.linearVelocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;

        //다양한 위치에서 훈련할 수 있도록 에이전트 위치를 랜덤하게 재설정
        //true일 경우 꽃 앞에 위치하도록 에이전트 위치를 재설정
        bool infrontOfFlower = true;
        if (trainingMode)
        {
            //훈련모드에서 50% 확률로 꽃 앞에 위치하도록 에이전트 위치를 재설정
            infrontOfFlower = UnityEngine.Random.value > .5f;

            //새로운 랜덤 위치로 에이전트 위치를 재설정
            MoveToSafeRandomPosition(infrontOfFlower);
        }
        
        //새로운 랜덤 위치로 에이전트 위치를 재설정 후 가장 가까운 꽃을 찾음
        UpdateNearestFlower();
    }

    /// <summary>
    /// 에이전트 위치를 안전한 랜덤 위치로 재설정(어떤것과도 충돌하지 않는 위치)
    /// 만약 꽃 앞에 있다면 꽃 앞에 위치하도록 재설정
    /// </summary>
    /// <param name="infrontOfFlower">꽃 앞에서 장소를 선택할지 여부</param>
    /// <exception cref="NotImplementedException"></exception>
    private void MoveToSafeRandomPosition(bool infrontOfFlower)
    {
        bool safePositionFound = false;
        int attemptRemaining = 100; //무한루프 방지를 위해 최대 시도 횟수 제한
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        while (!safePositionFound && attemptRemaining > 0)
        {
            attemptRemaining--;

            if (infrontOfFlower) //꽃 앞에 위치할 때 
            {
                //랜덤한 꽃을 선택
                ML_Flower randomflowers = flowerArea.flowers[UnityEngine.Random.Range(0, flowerArea.flowers.Count)];
                //꽃 앞에 10~20cm 떨어진 위치를 랜덤하게 선택
                float distanceFromFlower = UnityEngine.Random.Range(0.1f, 0.2f);
                potentialPosition = randomflowers.transform.position + randomflowers.flowerUpVector * distanceFromFlower;
                //부리와 닿는 지점
                Vector3 toFlower = randomflowers.flowerCenterPosition - potentialPosition;
                //꽃을 바라보도록 회전
                potentialRotation = Quaternion.LookRotation(toFlower, Vector3.up);
            }
            else //꽃 앞이 아닐 때
            {
                //지면으로부터의 거리
                float height = UnityEngine.Random.Range(1.2f, 1.5f);
                //임의의 반경 설정
                float radius = UnityEngine.Random.Range(2f, 7f);
                //임의의 방향 선택
                Quaternion dir = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f, 180f), 0f);
                //높이와 반경, 방향을 고려해 잠재적 위치 선정
                potentialPosition = flowerArea.transform.position + Vector3.up * height + dir * Vector3.forward * radius;
                //임의의 회전 각도 선택
                float pitch = UnityEngine.Random.Range(-60f, 60f);
                float yaw = UnityEngine.Random.Range(-180f, 180f);
                potentialRotation = Quaternion.Euler(pitch, yaw, 0f);
            }
            //에이전트가 무언가와 충돌하는 경우
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 0.05f);
            //충돌이 없다면 안전한 위치로 판단
            safePositionFound = colliders.Length == 0;
        }
        Debug.Assert(safePositionFound, "안전한 위치를 찾지 못했습니다. 시도 횟수 초과");
        //에이전트 위치와 회전을 안전한 랜덤 위치로 재설정
        transform.position = potentialPosition;
        transform.rotation = potentialRotation;
    }
    /// <summary>
    /// 가장 가까운 꽃을 찾는 메서드
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void UpdateNearestFlower()
    {
        foreach (ML_Flower flower in flowerArea.flowers)
        {
            if (nearestFlower == null && flower.HasNectar)
            {
                // 가장 가까운 꽃이 아직 설정되지 않았고, 현재 꽃에 넥타르가 있다면 해당 꽃을 가장 가까운 꽃으로 설정    
                nearestFlower = flower;
            }
            else if (flower.HasNectar)
            {
                // 여기에 가장 가까운 꽃을 찾는 로직을 추가할 수 있습니다.
                float distancetoFlower = Vector3.Distance(flower.transform.position, beakTip.position);
                float distancetoNearestFlower = Vector3.Distance(nearestFlower.transform.position, beakTip.position);

                //꽃이 넥타르를 가지고 있지 않거나, 더 가까운 꽃이 있는 경우
                if (!nearestFlower.HasNectar || distancetoFlower < distancetoNearestFlower)
                {
                    nearestFlower = flower;
                }
            }
        }
    }

    /// <summary>
    /// 플레이어로부터의 입력 혹은 신경망으로부터 행동을 수신받아 ai가 결정을 내림
    /// Agent가 무엇을 해야할지 알려주는 역할을 하는 함수
    /// 원본은 float vectorAction[i] 였으나, 버전의 차이로 ActionBuffers actions로 변경됨
    /// 각 action은 다른 행동을 담당함
    /// Index 0 : x축 이동
    /// Index 1 : y축 이동
    /// Index 2 : z축 이동
    /// Index 3 : x축 회전
    /// Index 4 : y축 회전
    /// </summary>
    /// <param name="actions">행동을 취함</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        // 화면이 멈춰있다면 아무것도 하지않음
        if (frozen) return;

        //float moveX = actions.ContinuousActions[0];
        //float moveY = actions.ContinuousActions[1];
        //float moveZ = actions.ContinuousActions[2];

        //이동 벡터 계산
        Vector3 move = new Vector3(actions.ContinuousActions[0], actions.ContinuousActions[1], actions.ContinuousActions[2]);
        //이동 방향으로 힘을 부여
        agentRb.AddForce(move * moveForce);

        //회전 처리 - 떨림방지 및 부드러운 회전
        Vector3 rotationVector = transform.rotation.eulerAngles;

        //축 회전 계산
        float pitchChange = actions.ContinuousActions[3];
        float yawChange = actions.ContinuousActions[4];

        //부드러운 회전 변화량 계산
        smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
        smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);

        //뒤집히는 것을 방지하기 위한 축 계산
        float pitch = rotationVector.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);

        float yaw = rotationVector.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

        //계산된 새 각도를 적용
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
    /// <summary>
    /// 관찰 결과를 수집하는 함수
    /// 환경으로부터 벡터값 수집 및 브레인으로 전달
    /// </summary>
    /// <param name="sensor">벡터 값</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //만약 가장 가까운 꽃이 없으면, 관찰하고 있는 개수만큼의 빈 배열을 리턴해준다.
        if (nearestFlower == null)
        {
            sensor.AddObservation(new float[10]);
            return;
        }
        //에이전트의 회전을 관찰 - 4개 관찰
        sensor.AddObservation(transform.localRotation.normalized);

        //가장 가까운 꽃을 향하는 부리로부터 벡터를 얻어옴 
        Vector3 toFlower = nearestFlower.flowerCenterPosition - beakTip.position;

        //정규화된 벡터가 가리키는 가장 가까운 꽃 - 변수 3개 관찰
        sensor.AddObservation(toFlower.normalized);

        //부리 끝이 꽃의 앞에 있는지 내적을 관찰 - 변수 1개 관찰
        //+1은 꽃의 앞을, -1은 꽃의 뒤를 의미함
        sensor.AddObservation(Vector3.Dot(toFlower.normalized, -nearestFlower.flowerUpVector.normalized));

        //부리가 가리키는 방향이 꽃의 앞인지 내적을 관찰 - 변수 1개 관찰
        //+1이면 부리가 꽃의 앞쪽을 가리킴, -1이면 뒤쪽을 가리킴
        sensor.AddObservation(Vector3.Dot(beakTip.forward.normalized, -nearestFlower.flowerUpVector.normalized));

        //부리의 끝부분과 꽃 사이의 상대적인 거리를 관찰 - 변수 1개 관찰
        sensor.AddObservation(toFlower.magnitude / ML_FlowerArea.AreaDiameter);
    }
    /// <summary>
    /// 에이전트의 동작 매개변수에서 동작 유형이 "휴리스틱 전용"으로 설정된 경우 이 함수가 호출.
    /// 반환값은 신경망을 사용하는 대신에 다음 함수에 입력됨
    /// <see cref="OnActionReceived(ActionBuffers)"> 에 들어갈 입력값을 이곳에서 지정해준다.
    /// </summary>
    /// <param name="actionsOut">그리고 행동 배열을 출력함</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //모든 움직임/ 회전에 대한 플레이스홀더 만들기
        Vector3 forward = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        float pitch = 0f;
        float yaw = 0f;

        //키보드 이동 및 회전 입력값을 받아 변환
        //모든 값은 -1 ~ 1 사이
        if (Input.GetKey(KeyCode.W)) forward = transform.forward;
        else if (Input.GetKey(KeyCode.S)) forward = -transform.forward;

        if (Input.GetKey(KeyCode.A)) left = -transform.right;
        else if (Input.GetKey(KeyCode.D)) left = transform.right;

        if (Input.GetKey(KeyCode.E)) up = transform.up;
        else if (Input.GetKey(KeyCode.C)) up = -transform.up;

        //pitch up/down
        if (Input.GetKey(KeyCode.UpArrow)) pitch = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) pitch = -1f;
        //turn left/right
        if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) yaw = 1f;

        //이동 벡터를 정규화 및 결합
        Vector3 combine = (forward + left + up).normalized;
        // ContinuousActions 버퍼 가져오기   ContinuousActions = 연속형, DiscreteActions = 이산형
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // 행동 버퍼에 정규화된 이동값 할당
        continuousActions[0] = combine.x;
        continuousActions[1] = combine.y;
        continuousActions[2] = combine.z;
        // 회전값 전달
        continuousActions[3] = pitch;
        continuousActions[4] = yaw;
    }
    public void FreezeAgent()
    {
        frozen = true;
        agentRb.Sleep();
    }
    public void UnFreezeAgent()
    {
        frozen = false;
        agentRb.WakeUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerEngerOrStay(other);
    }
    private void OnTriggerStay(Collider other)
    {
        TriggerEngerOrStay(other);
    }
    
    private void TriggerEngerOrStay(Collider collider)
    {
        //에이전트가 넥타르와 접촉했는지 확인
        if (collider.CompareTag("nectar"))
        {
            Vector3 closestPointBeakTip = collider.ClosestPoint(beakTip.position);

            //부리 끝과 가장 가까운 지점과의 충돌인지 체크, 두 지점사이의 거리가 부리끝의 반경보다 짧아야한다.
            if (Vector3.Distance(beakTip.position, closestPointBeakTip) < beatTipRadius)
            {
                //꽃에 넥타르와 충돌
                ML_Flower flower = flowerArea.GetFlowerFromNectar(collider);

                //꿀 0.1을 얻으려 시도, 0.2초마다 주기적으로 시도함
                float nectarReceived = flower.Feed(0.1f);

                NectarObtained += nectarReceived;

                if (trainingMode)
                {
                    //얻은 꿀을 보상으로 계산, 보상은 꿀을 얻는 행위를 장려한다
                    //보상은 일관성이 있어야 에이전트가 혼동하지 않는다. 예를 들어 같은 행위를 하는데
                    //보상이 20점, 1점, -10점 이런식으로 주어지면 에이전트는 해당 행동을 하는것에 혼란을 느끼게 된다.
                    float bonus = .02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -nearestFlower.flowerUpVector.normalized));
                    AddReward(.01f + bonus);
                }
                //꽃이 비어있을 경우, 가장 가까운 꽃을 다시 찾음
                if (!flower.HasNectar)
                {
                    UpdateNearestFlower();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(trainingMode && collision.collider.CompareTag("boundary"))
        {
            //영역을 벗어나 벽에 닿으면 보상 감소, 감소량이 너무 크면 에이전트는 행동하지 않을 수도 있다.
            AddReward(-.5f);
        }
    }

    private void Update()
    {
        //부리 끝으로부터 가까운 꽃까지 선을 그린다.
        if (nearestFlower != null)
        {
            Debug.DrawLine(beakTip.position,nearestFlower.flowerCenterPosition, Color.green);
        }
    }
    private void FixedUpdate()
    {
        //가장 가까운 꽃의 꿀이 없는 시나리오를 회피
        if (nearestFlower != null && !nearestFlower.HasNectar)
            UpdateNearestFlower();
    }
}
