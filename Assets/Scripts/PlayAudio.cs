using UnityEngine;
using UnityEngine.UI;

public class Audio_Play : MonoBehaviour
{
    private AudioSource Audio;//AudioSource������
    bool isAudioStart = false; //�ȍĐ��̔���
    void Start()
    {
        Audio = GetComponent<AudioSource>();//AudioSource�̎擾
        Audio.Play();//AudioSource���Đ�
        isAudioStart = true;//�Ȃ̍Đ��𔻒�
    }
    void Update()
    {
        if (!Audio.isPlaying && isAudioStart)
        //�Ȃ��Đ�����Ă��Ȃ��A�����Ȃ̍Đ����J�n����Ă��鎞
        {
            Destroy(gameObject);//�I�u�W�F�N�g������
        }
    }
}