using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCube : Photon.MonoBehaviour, IPunObservable
{
    private const float lerpSpeed = 10f;

    private Vector3 correctPlayerPos;
    private Quaternion correctPlayerRot;

    void Start()
    {
        correctPlayerPos = transform.position;
        correctPlayerRot = transform.rotation;
    }

    void Update()
    {
        if (PhotonNetwork.connected == false)
            return;

        if (photonView.isMine == false)
        {
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * lerpSpeed);            
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting == true)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

