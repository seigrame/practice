using System;
using System.Collections.Generic;
using static System.Console;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

/* 오후에 할 일
 * 1. 클래스로 분리 하기
 * 2.폭탄이 터질때 벽을 마주하면 한칸만, 벽이 없으면 파워최대치만큼 닿게 만든다. 현재상태는 폭탄의 최대치만큼 벽을 부수고 있기때문에 블록이 너무 금방 사라지는 문제가 있다. <= 조금 더 고민하기
 * 3.폭탄 갯수 증가 아이템 만들기
*/


//브레이크 포인트 = 브레이크 포인트를 걸고 f5를 누르면 실행 f10을 누르면 바로 다음 그 아랫줄로, f11을 누르면 함수안으로 타고 들어간다.
namespace advenced
{ 
    class Program
    {
        //맵 구성  0. 이동 가능 타일 1.파괴 가능한 벽 2, 파괴 불가능한 벽 3. 종료지점
        //4. 폭탄  5. 폭탄 파워 증가 6. 벽밀기 아이템
        static int[,] map = new int[20, 20]
            {
                 { 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                 { 2,0,1,0,0,0,1,0,0,0,1,1,0,0,0,0,0,1,0,2},
                 { 2,0,7,1,1,0,1,1,1,0,0,0,0,1,7,1,0,1,0,2},
                 { 2,0,0,0,0,0,0,0,0,0,1,1,1,7,1,1,0,1,0,2},
                 { 2,7,7,0,7,7,1,1,1,1,1,1,7,1,1,1,0,0,0,2},
                 { 2,0,0,0,0,0,0,0,0,7,1,0,1,0,0,0,0,1,1,2},
                 { 2,1,1,1,1,1,1,1,0,7,1,0,1,0,1,7,7,7,1,2},
                 { 2,0,1,0,0,0,1,1,0,7,1,0,0,0,1,1,1,1,1,2},
                 { 2,0,0,0,1,0,0,0,0,1,1,1,1,0,0,0,0,0,1,2},
                 { 2,1,1,0,1,1,7,7,7,1,1,1,1,7,7,0,1,1,1,2},
                 { 2,1,1,0,0,0,1,1,0,0,0,0,1,7,1,0,7,1,1,2},
                 { 2,1,1,0,1,0,1,1,0,1,1,0,1,7,1,0,1,7,1,2},
                 { 2,0,0,0,1,1,1,0,0,1,1,0,1,7,1,0,1,1,7,2},
                 { 2,0,1,1,1,1,0,0,1,1,1,0,1,0,0,0,0,0,0,2},
                 { 2,0,7,1,1,1,7,1,7,1,1,0,1,0,1,1,1,1,1,2},
                 { 2,0,7,1,0,0,0,0,1,1,1,0,1,0,1,1,1,1,1,2},
                 { 2,0,1,1,0,1,1,0,7,1,1,0,0,0,0,0,0,1,1,2},
                 { 2,0,1,1,0,1,7,0,0,0,1,1,1,0,1,1,0,1,1,2},
                 { 2,0,0,0,0,1,1,1,7,0,0,0,0,0,1,1,0,0,3,2},
                 { 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2}
            };

        static bool gameState = false;
        static Stopwatch stopwatch = new Stopwatch();
        static long Now { get; set; } = 0;                      //현재시간
        static long Before { get; set; } = 0;                   //이전시간
        static int frameTime = 100;                            //프레임타임 1초

        static ConsoleKeyInfo c;
        static int defaultBombCount = 0;
        static int MaxBombCount = 2;
        static PositionInfo PlayerPos = new PositionInfo(1, 1);
        static PositionInfo[] bomb = new PositionInfo[5];

        static Random rand = new Random();
        static int bombPower;
        static bool isPush;


        static DateTime lastActionTime = DateTime.Now;
        static void Main(string[] args)
        {

            int[] count = new int[3];
            Init();
            //PositionInfo[] bomb = new PositionInfo[5];

            SetCursorPosition(20, 20);
            //BombCheck();
            while (gameState)
            {

                Update(ref count, ref PlayerPos);
                Render(count, PlayerPos);
            }
            Release();
        }

        static void Init()
        {
            gameState = true;
            stopwatch.Start();
            InitArr();
            bombPower = 1;
            isPush = false;
            SetCursorPosition(50, 2);
            Write("왼쪽 : ←  오른쪽 :  →  위 : ↑  아래 : ↓");
            SetCursorPosition(50, 5);
            Write("폭탄 설치 : z  폭탄 터트리기 : x  벽 밀기 : c ");
        }

        static void Update(ref int[] count, ref PositionInfo playerPos)
        {
            while (!isRenderTime())
            {
                KeyInput(ref playerPos);
                //플레이어의 현재 좌표 표시
                SetCursorPosition(50, 20);
                Write("X : " + string.Format("{0:D2}", playerPos.X) + ", Y : " + string.Format("{0:D2}", playerPos.Y));
            }
            count[2]++;
            if (count[2] == 10)
            {
                count[1]++;
                count[2] = 0;
            }
            if (count[1] == 60)
            {
                count[0]++;
                count[1] = 0;
            }

            if (map[playerPos.X, playerPos.Y] == 3)
            {
                WriteLine("게임에서 승리하셨습니다");
                gameState = false;
            }
            SetCursorPosition(50, 19);
            Write("현재 폭탄의 파워 : " + bombPower.ToString());
            SetCursorPosition(55, 19);
            Write("벽밀기 : ");
            if (isPush)
                Write("ON");
            else
                Write("OFF");
        }

        static void Render(int[] count, PositionInfo playerPos)
        {
            CursorVisible = false;
            SetCursorPosition(17, 0);
            Write(count[0] + ":" + count[1] + ":" + count[2]);
            SetCursorPosition(0, 1);
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (PlayerPos.X == j && PlayerPos.Y == i)
                    {
                        Write("●");
                    }
                    else if (map[i, j] == 0)
                    {
                        Write(" ");
                    }
                    else if (map[i, j] == 1)
                    {
                        Write("▦");
                    }
                    else if (map[i, j] == 2)
                    {
                        Write("▧");
                    }
                    else if (map[i, j] == 3)
                    {
                        Write("♪");
                    }
                    else if (map[i, j] == 4)
                    {
                        Write("◎");
                    }
                    else if (map[i, j] == 5)
                    {
                        Write("♧");
                    }
                    else if (map[i, j] == 6)
                    {
                        Write("※");
                    }
                    else if (map[i, j] == 7)
                    {
                        Write("▥");
                    }

                    else if (map[i, j] == 8)
                    {
                        Write("ⓟ");
                    }
                }
                WriteLine();
            }
        }

        static void Release()
        {
            stopwatch.Stop();
        }

        static bool isRenderTime()
        {
            //밀리세컨트 단위로 시간을 재고
            Now = stopwatch.ElapsedMilliseconds;
            //만약 지금 시간에서 이전 시간을 뺐을때, 프레임 타임을 넘어선다면 
            if (Now - Before > frameTime)
            {
                //프레임 타임을 이전 시간에 기록 true를 반환한다 => 0.1마다 true 반환 
                Before += frameTime;
                return true;
            }
            return false;
        }

        static void KeyInput(ref PositionInfo playerPos)
        {
            if (KeyAvailable)
            {
                c = ReadKey();
                switch (c.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (playerPos.Y - 1 >= 1)
                        {
                            //이동할 수 있는 곳인지 먼저 체크함(1,2,7 번은 벽/ 4번은 폭탄)
                            if (map[playerPos.Y - 1, playerPos.X] != 1 &&
                                map[playerPos.Y - 1, playerPos.X] != 2 &&
                                map[playerPos.Y - 1, playerPos.X] != 4 &&
                                map[playerPos.Y - 1, playerPos.X] != 7)
                            {
                                --playerPos.Y;
                            }
                            //벽밀기가 가능하고 바로 윗칸이 벽일 경우
                            else if (isPush == true && map[playerPos.Y - 1, playerPos.X] == 1)
                            {
                                //위의 윗칸이 0보다 클 경우 맵이 생성되어 있다는 의미이고
                                if (playerPos.Y - 2 >= 0)
                                {
                                    //위의 윗칸이 빈 공간인지 체크한다.
                                    if (map[playerPos.Y - 2, playerPos.X] == 0)
                                    {
                                        //위의 윗칸은 벽으로, 윗칸은 빈 공간으로 만들고 플레이어를 이동시킨다..
                                        map[playerPos.Y - 2, playerPos.X] = 1;
                                        map[playerPos.Y - 1, playerPos.X] = 0;
                                        --playerPos.Y;
                                    }
                                    //벽의 경우
                                    else if (map[playerPos.Y - 2, playerPos.X] == 1)
                                    {
                                        return;
                                    }
                                }
                            }
                            //벽밀기가 불가능할 경우
                            //아이템을 습득하면 아이템이 있던 자리는 빈칸으로
                            if (AddItem(map[playerPos.Y, playerPos.X]))
                            {
                                map[playerPos.Y, playerPos.X] = 0;
                            }

                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (playerPos.Y + 1 < 20)
                        {
                            if (map[playerPos.Y + 1, playerPos.X] != 1 &&
                                map[playerPos.Y + 1, playerPos.X] != 2 &&
                                map[playerPos.Y + 1, playerPos.X] != 4 &&
                                map[playerPos.Y + 1, playerPos.X] != 7)
                            {
                                ++playerPos.Y;
                            }
                            else if (isPush == true && map[playerPos.Y + 1, playerPos.X] == 1)
                            {
                                //아래의 아랫칸이 0보다 클 경우 맵이 생성되어 있다는 의미이고
                                if (playerPos.Y + 2 < 20)
                                {
                                    //아래의 아랫칸이 빈 공간인지 체크한다.
                                    if (map[playerPos.Y + 2, playerPos.X] == 0)
                                    {
                                        //아래의 아랫칸은 벽으로, 아랫칸은 빈 공간으로 만들고 플레이어를 이동시킨다..
                                        map[playerPos.Y + 2, playerPos.X] = 1;
                                        map[playerPos.Y + 1, playerPos.X] = 0;
                                        ++playerPos.Y;
                                    }
                                    //벽의 경우
                                    else if (map[playerPos.Y + 2, playerPos.X] == 1)
                                    {
                                        return;
                                    }
                                }
                            }
                            //아이템을 습득하면 아이템이 있던 자리는 빈칸으로
                            if (AddItem(map[playerPos.Y, playerPos.X]))
                            {
                                map[playerPos.Y, playerPos.X] = 0;
                            }
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (playerPos.X + 1 < 20)
                        {
                            if (map[playerPos.Y, playerPos.X + 1] != 1 &&
                                map[playerPos.Y, playerPos.X + 1] != 2 &&
                                map[playerPos.Y, playerPos.X + 1] != 4 &&
                                map[playerPos.Y, playerPos.X + 1] != 7)
                            {
                                ++playerPos.X;
                            }
                            else if (isPush == true && map[playerPos.Y, playerPos.X + 1] == 1)
                            {
                                //우측의 우측이 0보다 클 경우 맵이 생성되어 있다는 의미이고
                                if (playerPos.X + 2 < 20)
                                {
                                    //우측의 우측이 빈 공간인지 체크한다.
                                    if (map[playerPos.Y, playerPos.X + 2] == 0)
                                    {
                                        //우측의 우측칸은 벽으로, 우측칸은 빈 공간으로 만들고 플레이어를 이동시킨다.
                                        map[playerPos.Y, playerPos.X + 2] = 1;
                                        map[playerPos.Y, playerPos.X + 1] = 0;
                                        ++playerPos.X;
                                    }
                                    //벽의 경우
                                    else if (map[playerPos.Y, playerPos.X + 2] == 1)
                                    {
                                        return;
                                    }
                                }
                            }
                            //아이템을 습득하면 아이템이 있던 자리는 빈칸으로
                            if (AddItem(map[playerPos.Y, playerPos.X]))
                            {
                                map[playerPos.Y, playerPos.X] = 0;
                            }
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (playerPos.X - 1 >= 1)
                        {
                            if (map[playerPos.Y, playerPos.X - 1] != 1 &&
                                map[playerPos.Y, playerPos.X - 1] != 2 &&
                                map[playerPos.Y, playerPos.X - 1] != 4 &&
                                map[playerPos.Y, playerPos.X - 1] != 7)
                            {
                                --playerPos.X;
                            }
                            else if (isPush == true && map[playerPos.Y, playerPos.X - 1] == 1)
                            {
                                //좌측의 좌측이 0보다 클 경우 맵이 생성되어 있다는 의미이고
                                if (playerPos.X - 2 >= 0)
                                {
                                    //좌측의 좌측이 빈 공간인지 체크한다.
                                    if (map[playerPos.Y, playerPos.X - 2] == 0)
                                    {
                                        //좌측의 좌측칸은 벽으로, 좌측칸은 빈 공간으로 만들고 플레이어를 이동시킨다.
                                        map[playerPos.Y, playerPos.X - 2] = 1;
                                        map[playerPos.Y, playerPos.X - 1] = 0;
                                        --playerPos.X;
                                    }
                                    //벽의 경우
                                    else if (map[playerPos.Y, playerPos.X - 2] == 1)
                                    {
                                        return;
                                    }
                                }
                            }
                            //아이템을 습득하면 아이템이 있던 자리는 빈칸으로
                            if (AddItem(map[playerPos.Y, playerPos.X]))
                            {
                                map[playerPos.Y, playerPos.X] = 0;
                            }
                        }
                        break;
                    case ConsoleKey.Z:
                        {
                            //맵 플레이어위치, 폭탄 위치, 폭탄갯수
                            CreateBomb(map, playerPos, bomb, ref defaultBombCount);
                        }
                        break;
                    case ConsoleKey.X:
                        {
                            Fire(map, ref playerPos, bomb, ref defaultBombCount);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        static void CreateBomb(int[,] map, PositionInfo PlayerPos, PositionInfo[] bomb, ref int bombCount)
        {
            if (bombCount == MaxBombCount)
            {
                return;
            }

            for (int i = 0; i < bombCount; i++)
            {
                //폭탄이 놓인 위치와 폭탄을 생성할 위치가 동일하다면 생성하지 않음
                if (PlayerPos.X == bomb[i].X && PlayerPos.Y == bomb[i].Y)
                {
                    return;
                }
            }
            //BOMB[0~4]까지 위치를 기억함
            bomb[bombCount] = PlayerPos;
            bombCount++;
            map[PlayerPos.Y, PlayerPos.X] = 4;
        }

        static void Fire(int[,] map, ref PositionInfo PlayerPos, PositionInfo[] bomb, ref int bombCount)
        {
            for (int i = 0; i < bombCount; i++)
            {
                //폭탄이 터진자리는 공백으로
                map[bomb[i].Y, bomb[i].X] = 0;
                //폭탄의 파워만큼
                for (int j = 1; j <= bombPower; j++)
                {
                    //맵이 있는지 체크
                    if (bomb[i].Y - j >= 0)
                    {
                        //파워만큼의 벽이 존재한다면 
                        if (map[bomb[i].Y - j, bomb[i].X] == 1)
                        {
                            //아이템을 생성하거나
                            if (rand.Next(100) < 20)
                            {
                                int percent = rand.Next(100);
                                if (percent < 70)
                                {
                                    map[bomb[i].Y - j, bomb[i].X] = 8;
                                }
                                else if (percent < 90)
                                {
                                    map[bomb[i].Y - j, bomb[i].X] = 5;
                                }
                                else
                                {
                                    map[bomb[i].Y - j, bomb[i].X] = 6;
                                }
                            }
                            //벽을 빈공간으로 바꿈
                            else
                            {
                                map[bomb[i].Y - j, bomb[i].X] = 0;
                            }
                        }
                        //아이템이 존재한다면 삭제
                        else if (map[bomb[i].Y - j, bomb[i].X] == 5 ||
                                map[bomb[i].Y - j, bomb[i].X] == 6 ||
                                map[bomb[i].Y, bomb[i].X + j] == 8)
                        {
                            map[bomb[i].Y - j, bomb[i].X] = 0;
                        }
                        //2번 깨야하는 블럭
                        else if (map[bomb[i].Y - j, bomb[i].X] == 7)
                        { 
                            map[bomb[i].Y - j, bomb[i].X] = 1; 
                        }
                        //플레이어가 폭탄에 맞았을경우 시작점으로
                        if (PlayerPos.X == bomb[i].X && PlayerPos.Y == bomb[i].Y - j)
                        {
                            PlayerPos.X = 1;
                            PlayerPos.Y = 1;
                        }
                    }
                    if (bomb[i].Y + j < 19)
                    {   //벽이면 빈공간으로 
                        if (map[bomb[i].Y + j, bomb[i].X] == 1)
                        {
                            if (rand.Next(100) < 20)
                            {
                                int percent = rand.Next(100);
                                if (percent < 70)
                                {
                                    map[bomb[i].Y + j, bomb[i].X] = 8;
                                }
                                else if (percent < 90)
                                {
                                    map[bomb[i].Y + j, bomb[i].X] = 5;
                                }
                                else
                                {
                                    map[bomb[i].Y + j, bomb[i].X] = 6;
                                }
                            }
                            //벽을 빈공간으로 바꿈
                            else
                            {
                                map[bomb[i].Y + j, bomb[i].X] = 0;
                            }
                        }
                        else if (map[bomb[i].Y + j, bomb[i].X] == 5 ||
                                 map[bomb[i].Y + j, bomb[i].X] == 6 ||
                                 map[bomb[i].Y, bomb[i].X + j] == 8)
                        {
                            map[bomb[i].Y + j, bomb[i].X] = 0;
                        }
                        else if (map[bomb[i].Y + j, bomb[i].X] == 7)
                        {
                            map[bomb[i].Y + j, bomb[i].X] = 1;
                        }
                        //플레이어가 폭탄에 맞았을경우 시작점으로
                        if (PlayerPos.X == bomb[i].X && PlayerPos.Y == bomb[i].Y + j)
                        {
                            PlayerPos.X = 1;
                            PlayerPos.Y = 1;
                        }
                    }
                    if (bomb[i].X - j >= 0)
                    {   //벽이면 빈공간으로 
                        if (map[bomb[i].Y, bomb[i].X - j] == 1)
                        {
                            if (rand.Next(100) < 20)
                            {
                                int percent = rand.Next(100);
                                if (percent < 70)
                                {
                                    map[bomb[i].Y, bomb[i].X - j] = 8;
                                }
                                else if (percent < 90)
                                    map[bomb[i].Y, bomb[i].X - j] = 5;
                                else
                                {
                                    map[bomb[i].Y, bomb[i].X - j] = 6;
                                }
                            }
                            //벽을 빈공간으로 바꿈
                            else
                            {
                                map[bomb[i].Y, bomb[i].X - j] = 0;
                            }
                        }
                        else if (map[bomb[i].Y, bomb[i].X - j] == 5 ||
                            map[bomb[i].Y, bomb[i].X - j] == 6 ||
                            map[bomb[i].Y, bomb[i].X + j] == 8)
                        {
                            map[bomb[i].Y, bomb[i].X - j] = 0;
                        }
                        else if (map[bomb[i].Y, bomb[i].X - j] == 7)
                        {
                            map[bomb[i].Y, bomb[i].X - j] = 1;
                        }
                        if (PlayerPos.X == bomb[i].X - j && PlayerPos.Y == bomb[i].Y)
                        {
                            PlayerPos.X = 1;
                            PlayerPos.Y = 1;
                        }
                    }
                    if (bomb[i].X + j < 19)
                    {   //벽이면 빈공간으로 
                        if (map[bomb[i].Y, bomb[i].X + j] == 1)
                        {
                            if (rand.Next(100) < 20)
                            {
                                int percent = rand.Next(100);
                                if (percent < 70)
                                {
                                    map[bomb[i].Y, bomb[i].X + j] = 8;
                                }
                                else if(percent < 90)
                                    map[bomb[i].Y, bomb[i].X + j] = 5;
                                else
                                {
                                    map[bomb[i].Y, bomb[i].X + j] = 6;
                                }
                            }
                            //벽을 빈공간으로 바꿈
                            else
                                map[bomb[i].Y, bomb[i].X + j] = 0;

                        }
                        else if (map[bomb[i].Y, bomb[i].X + j] == 5 ||
                            map[bomb[i].Y, bomb[i].X + j] == 6 ||
                            map[bomb[i].Y, bomb[i].X + j] == 8)
                        {
                            map[bomb[i].Y, bomb[i].X + j] = 0;
                        }
                        else if (map[bomb[i].Y, bomb[i].X + j] == 7)
                        {
                            map[bomb[i].Y, bomb[i].X + j] = 1;
                        }
                        if (PlayerPos.X == bomb[i].X + j && PlayerPos.Y == bomb[i].Y)
                        {
                            PlayerPos.X = 1;
                            PlayerPos.Y = 1;
                        }
                    }
                }
            }
            bombCount = 0;
        }

        static void InitArr()
        {
            for (int i = 0; i < 5; i++)
            {
                bomb[i] = new PositionInfo(0, 0);
            }
        }

        static bool AddItem(int ItemType)
        {
            if (ItemType == 5)
            {
                if (bombPower < 5)
                    bombPower++;
                return true;
            }
            else if (ItemType == 6)
            {
                isPush = true;
                return true;
            }

            //폭탄 증가 아이템 만들기 
            else if (ItemType == 8)
            {
                if (MaxBombCount < 5)
                {
                    MaxBombCount++;
                }
                return true;
            }
            return false;
        }
    }
}
