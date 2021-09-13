﻿using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace QRTracking
{
    [RequireComponent(typeof(QRTracking.SpatialGraphNodeTracker))]
    public class QRCode : MonoBehaviour
    {
        public Microsoft.MixedReality.QR.QRCode qrCode;
      

        public float PhysicalSize { get; private set; }
        public string CodeText { get; private set; }
        /*
        private TextMesh QRID;
        private TextMesh QRNodeID;
        private TextMesh QRText;
        private TextMesh QRVersion;
        private TextMesh QRTimeStamp;
        private TextMesh QRSize;
        private GameObject QRInfo;
        private bool validURI = false;
        private bool launch = false;
        private System.Uri uriResult;
        */
        private long lastTimeStamp = 0;

          [SerializeField]
        private GameObject qrMiddle;

        [SerializeField]
        Transform qrDeployPoint;
        public Transform QRDeployPoint { get{ return qrDeployPoint; } }

        // Use this for initialization
        void Start()
        {
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSideLength;

            /*
            // utf-8 인코딩
            string QRData = qrCode.Data;

            byte[] bytesForEncoding = Encoding.UTF8.GetBytes(QRData);

            string encodedString = System. Convert.ToBase64String(bytesForEncoding);



            // utf-8 디코딩

            byte[] decodedBytes = System.Convert.FromBase64String(encodedString);

            string decodedString = Encoding.UTF8.GetString(decodedBytes);


            CodeText = decodedString;
            */

            CodeText = qrCode.Data;

            /*
            QRInfo = gameObject.transform.Find("QRInfo").gameObject;
            QRID = QRInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            QRNodeID = QRInfo.transform.Find("QRNodeID").gameObject.GetComponent<TextMesh>();
            QRText = QRInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            QRVersion = QRInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            QRTimeStamp = QRInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            QRSize = QRInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();

            QRID.text = "Id:" + qrCode.Id.ToString();
            QRNodeID.text = "NodeId:" + qrCode.SpatialGraphNodeId.ToString();
            QRText.text = CodeText;
            

            if (System.Uri.TryCreate(CodeText, System.UriKind.Absolute,out uriResult))
            {
                validURI = true;
                QRText.color = Color.blue;
            }

            QRVersion.text = "Ver: " + qrCode.Version;
            QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";
            QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
            QRTimeStamp.color = Color.yellow;
            Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);
            */
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                /*
                QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";

                QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
                QRTimeStamp.color = QRTimeStamp.color==Color.yellow? Color.white: Color.yellow;
                */
                PhysicalSize = qrCode.PhysicalSideLength;
                //  Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " Time = " + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));
                
                qrMiddle.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                qrMiddle.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, PhysicalSize /*1f*/);
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
             //   QRInfo.transform.localScale = new Vector3(PhysicalSize/0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);
            }
        }



        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
            /*
            if (launch)
            {
                launch = false;
                LaunchUri();
            }
            */
        }

        void LaunchUri()
        {
#if WINDOWS_UWP
            // Launch the URI
            //    UnityEngine.WSA.Launcher.LaunchUri(uriResult.ToString(), true);
#endif
        }

        public void OnInputClicked()
        {/*
            if (validURI)
            {
                launch = true;
            }
            */
// eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }


    }
}