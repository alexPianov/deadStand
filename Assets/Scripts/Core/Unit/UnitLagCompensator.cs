using UnityEngine;
using Photon.Pun;

namespace NukeFactory
{
    public class UnitLagCompensator : MonoBehaviourPun, IPunObservable
    {
        public bool isActive;
        
        [Header("Synchronize")]
        private Vector3 latestPos = Vector3.zero;
        private Quaternion latestRot = Quaternion.identity;

        [Header("Compensation")]
        private float currentTime = 0;
        private double currentPacketTime = 0;
        private double lastPacketTime = 0;
        private Vector3 positionAtLastPacket = Vector3.zero;
        private Quaternion rotationAtLastPacket = Quaternion.identity;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void Start()
        {
            if (!PhotonNetwork.InRoom) Destroy(this);
            Active(true);
        }

        public void Active(bool state)
        {
            isActive = state;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!photonView) return;
            
            if(!isActive) return;

            if (stream.IsWriting)
            {
                //We own this player: send the others our data
                stream.SendNext(_transform.rotation);
                stream.SendNext(_transform.position);

            }
            else
            {
                //Network player, receive data
                latestRot = (Quaternion)stream.ReceiveNext();
                latestPos = (Vector3)stream.ReceiveNext();

                //Lag compensation
                currentTime = 0.0f;
                lastPacketTime = currentPacketTime;
                currentPacketTime = info.SentServerTime;

                positionAtLastPacket = _transform.position;
                rotationAtLastPacket = _transform.rotation;
            }
        }

        void Update()
        {
            if (!_transform) return;
            if (!photonView) return;
            if (!isActive) return;
            if (photonView.IsMine) return;
            
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;

            _transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot,
                (float)(currentTime / timeToReachGoal));

            _transform.position = Vector3.Lerp(positionAtLastPacket, latestPos,
                (float)(currentTime / timeToReachGoal));

        }

        private void Callbacks()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                PhotonStream PS = new PhotonStream(true, null);
                PhotonMessageInfo PI = new PhotonMessageInfo();
                OnPhotonSerializeView(PS, PI);
            }
        }
    }
}