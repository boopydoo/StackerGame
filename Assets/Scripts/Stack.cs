using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stacker
{
    public class Stack : MonoBehaviour
    {
        [SerializeField]
        GameObject[] mainStack;

        const float START_BOUND_SIZE = 8.0f;
        const float ERROR_MARGIN = 0.2f;
        const int COMBO_MIN = 10;
        const float COMBO_GAIN_SIZE = 0.2f;

        bool gameOver = false;

        [SerializeField]
        Vector2 stackBound;

        [SerializeField]
        int scoreCount = 0;
        [SerializeField]
        int combo = 0;
        [SerializeField]
        int activeIndex;

        [SerializeField]
        float blockSpeed = 2;
        [SerializeField]
        float blockMove = 1.5f;
        [SerializeField]
        bool isMovingOnX = true;
        [SerializeField]
        float lastBlockPosXZ;
        [SerializeField]
        Vector3 lastBlockPos;

        [SerializeField]
        Vector3 newStackPos;
        [SerializeField]
        float stackSpeed = 8;

        Vector3 onPlacePos;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(Vector3.up + (transform.position + lastBlockPos), new Vector3(stackBound.x, 1, stackBound.y));

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(onPlacePos, 0.5f);
        }

        void Awake()
        {
            stackBound = new Vector2(START_BOUND_SIZE, START_BOUND_SIZE);
            mainStack = new GameObject[transform.childCount];
        }

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                mainStack[i] = transform.GetChild(i).gameObject;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("space") && !gameOver)
            {
                onPlacePos = mainStack[activeIndex].transform.localPosition + Vector3.down * scoreCount;
                Debug.Log(mainStack[activeIndex].transform.localPosition + Vector3.down * scoreCount);
                if (PlaceBlock())
                {
                    scoreCount++;
                    SpawnBlock();
                }
                else
                {
                    EndGame();
                }
            }
            MoveBlock();

            //Move stack down
            transform.position = Vector3.Lerp(transform.position, newStackPos, stackSpeed * Time.deltaTime);
        }

        void SpawnBlock()
        {
            lastBlockPos = mainStack[activeIndex].transform.localPosition;
            newStackPos = new Vector3(0, -scoreCount, 0);

            activeIndex--;
            if (activeIndex < 0)
            {
                activeIndex = transform.childCount - 1;
            }
            blockMove = 1.5f;
            // mainStack[activeIndex].transform.localPosition = new Vector3(9, scoreCount, 0);
        }

        bool PlaceBlock()
        {
            Transform activeTransform = mainStack[activeIndex].transform;

            if (isMovingOnX)
            {
                //TODO
                //make sliced block outbound
                float deltaX = activeTransform.localPosition.x - lastBlockPos.x;
                stackBound = new Vector2(stackBound.x - Mathf.Abs(deltaX), stackBound.y);

                if (stackBound.x <= 0)
                {
                    return false;
                }

                activeTransform.localScale = new Vector3(stackBound.x, 1, stackBound.y);

                float offSetPos = deltaX / 2;
                activeTransform.localPosition -= new Vector3(offSetPos, 0, 0);
            }
            else
            {
                float deltaZ = activeTransform.localPosition.z - lastBlockPos.z;
                stackBound = new Vector2(stackBound.x, stackBound.y - Mathf.Abs(deltaZ));

                if (stackBound.y <= 0)
                {
                    return false;
                }

                activeTransform.localScale = new Vector3(stackBound.x, 1, stackBound.y);

                float offSetPos = deltaZ / 2;
                activeTransform.localPosition -= new Vector3(0, 0, offSetPos);
            }

            lastBlockPosXZ = (isMovingOnX) ? activeTransform.localPosition.x : activeTransform.localPosition.z;
            isMovingOnX = !isMovingOnX;

            return true;
        }

        void MoveBlock()
        {
            if (gameOver)
            {
                return;
            }
            blockMove += Time.deltaTime * blockSpeed;
            if (isMovingOnX)
            {
                mainStack[activeIndex].transform.localScale = new Vector3(stackBound.x, 1, stackBound.y);
                mainStack[activeIndex].transform.localPosition = new Vector3(Mathf.Sin(blockMove) * (START_BOUND_SIZE + 1), scoreCount, lastBlockPosXZ);
            }
            else
            {
                mainStack[activeIndex].transform.localScale = new Vector3(stackBound.x, 1, stackBound.y);
                mainStack[activeIndex].transform.localPosition = new Vector3(lastBlockPosXZ, scoreCount, Mathf.Sin(blockMove) * (START_BOUND_SIZE + 1));
            }
        }

        void EndGame()
        {
            Debug.Log("Game Over");
            gameOver = true;
            mainStack[activeIndex].AddComponent<Rigidbody>();
        }
    }
}