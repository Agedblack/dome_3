using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoulLike
{
    public class CameraController : MonoBehaviour
    {
        Animator anim_Player;
        public float horizontalspeed = 100.0f;//摄像机水平移动
        public float verticalSpeed = 50.0f;//摄像机垂直移动
        public float cameraDampValue = 0.2f;//摄像机追赶效果
        float tempEulerX;
        GameObject player;
        GameObject cameraHandle;
        GameObject camera_Player;//主摄像机
        Vector3 cameraDampVelocity;
        private void Awake()
        {
            cameraHandle = transform.parent.gameObject;
            player = cameraHandle.transform.parent.gameObject;
            anim_Player = player.GetComponentInChildren<Animator>();
            camera_Player = Camera.main.gameObject;
            Cursor.visible = false;//隐藏指针
            Cursor.lockState = CursorLockMode.Locked; //将光标锁定到游戏窗口的中心
        }
        private void Update()
        {
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");
            Vector3 tempModelEuler = anim_Player.transform.eulerAngles;//将anim的欧拉角取出来，存进去 在做完摄像机的移动后再复写回去 
            player.transform.Rotate(Vector3.up, mouseX * horizontalspeed * Time.deltaTime);//水平移动
            cameraHandle.transform.Rotate(Vector3.right, mouseY * -verticalSpeed * Time.deltaTime);//垂直移动

            tempEulerX -= mouseY * verticalSpeed * Time.deltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -10, 25);//限制垂直移动的范围
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);
            anim_Player.transform.eulerAngles = tempModelEuler;//复写

            camera_Player.transform.position = Vector3.SmoothDamp(camera_Player.transform.position, transform.position, ref cameraDampVelocity, 0.4f);
            camera_Player.transform.eulerAngles = transform.eulerAngles;

        }
    }
}
