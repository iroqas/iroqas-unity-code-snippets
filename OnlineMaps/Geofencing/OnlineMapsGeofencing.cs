using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Iroqas.OnlineMapsUtilities;

namespace Iroqas.OnlineMapsUtilities
{
    /// <summary>
    ///  Iroqas - September 2022 - v1.0 
    ///  
    ///     Geofencing utility for OnlineMaps Unity's asset.
    /// -----------------------------------------------------------------------------------------------
    /// 
    /// >> NOTE: Requires OnlineMaps asset.
    /// 
    /// This scripts brings geofencing functionality to OnlineMaps Unity's asset.
    /// 
    /// </summary>
    /// 
    public class OnlineMapsGeofencing : MonoBehaviour
    {
        [Header("External References")]
        public OnlineMapsPolygonManager polygonManager;

        public OnlineMapsLocationService locationService;

        // Dictionary containing the polygons in PoligonManager
        protected Dictionary<string, OnlineMapsDrawingPoly> fences
            = new Dictionary<string, OnlineMapsDrawingPoly>();

        // Dictionary containing fences where GPS position is in.
        protected Dictionary<string, OnlineMapsDrawingPoly> fencesContainingGPS
            = new Dictionary<string, OnlineMapsDrawingPoly>();


        // Event that triggers when GPS position enters in a fence.
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onGPSEnterFence
            = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();

        // Event that triggers when GPS position stays in a fence.
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onGPSStayFence
            = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();

        // Event that triggers when GPS position exit a fence.
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onGPSExitFence
            = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();

        private void Awake()
        {
            polygonManager = GetComponent<OnlineMapsPolygonManager>();
            locationService = GetComponent<OnlineMapsLocationService>();
        }

        protected void Start()
        {
            // Sets listeners to polygonManager's events.
            polygonManager.AddOnNewPolygonListener(OnNewPolygonListener, true);
            polygonManager.AddOnUpdatePolygonListener(OnUpdatePolygonListener);
            polygonManager.AddOnRemovePolygonListener(OnRemovePolygonListener);
            
            // Sets listeners to locationService events.
            if (locationService.useGPSEmulator)
            {
                StartCoroutine(GPSEmulatorCoroutine());
            }
            else
            {
                locationService.OnLocationChanged += CheckFences;
            }
        }

        /// <summary>
        /// GPS Emulator Coroutine. Is activated when GPS emulator is enabled.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator GPSEmulatorCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(.5f);
                if (this.isActiveAndEnabled)
                {
                    print("Ping: " + locationService.emulatorPosition);
                    CheckFences(locationService.emulatorPosition);
                }
                yield return new WaitForSeconds(.5f);
            }

        }

        /// <summary>
        /// Adds a listener to OnGPSEnterFence event. This event is triggered when GPS enters in a fence.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="getAllExistingElements"></param>
        public void AddOnGPSEnterFenceListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback, bool getAllExistingElements = false)
        {
            onGPSEnterFence.AddListener(callback);
        }

        /// <summary>
        /// Removes a listener to OnGPSEnterFence event.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnGPSEnterFenceListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onGPSEnterFence.RemoveListener(callback);
        }

        /// <summary>
        /// Adds a listener to OnGPSStay event. This event is triggered on each GPS Update, when GPS stays in a fence.
        /// </summary>
        /// <param name="callback"></param>
        public void AddOnGPSStayFenceListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onGPSStayFence.AddListener(callback);
        }

        /// <summary>
        /// Removes a listener to OnGPSStay event.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnGPSStayFenceListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onGPSStayFence.RemoveListener(callback);
        }

        /// <summary>
        /// Adds a listener to OnGPSExit event. This event is triggered, when GPS exits a fence.
        /// </summary>
        /// <param name="callback"></param>
        public void AddOnGPSExitFenceListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onGPSExitFence.AddListener(callback);
        }

        /// <summary>
        /// Removes a listener to OnGPSExit event. 
        /// </summary>
        /// <param name="callback"></param>

        public void RemoveOnGPSExitFenceListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onGPSExitFence.RemoveListener(callback);
        }

        /// <summary>
        /// Handles OnNewPolygon's events from polygonManager
        /// </summary>
        /// <param name="polygon"></param>
        protected void OnNewPolygonListener(KeyValuePair<string, OnlineMapsDrawingPoly> polygon)
        {
            fences.Add(polygon.Key, polygon.Value);
        }

        /// <summary>
        /// Handles OnRemovePolygon's events from polygonManager
        /// </summary>
        /// <param name="polygon"></param>
        protected void OnRemovePolygonListener(KeyValuePair<string, OnlineMapsDrawingPoly> polygon)
        {
            fences.Remove(polygon.Key);
        }

        /// <summary>
        /// Handles OnUpdatePolygon's events from polygonManager
        /// </summary>
        /// <param name="polygon"></param>
        protected void OnUpdatePolygonListener(KeyValuePair<string, OnlineMapsDrawingPoly> polygon)
        {
            fences[polygon.Key] = polygon.Value;
        }


        /// <summary>
        /// Checks if GPS position is contained in one or more fences.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool IsGPSInFence(string key)
        {
            return fencesContainingGPS.ContainsKey(key);
        }

        /// <summary>
        /// Checks if GPS position is contained in one or more fences. Performs event triggering.
        /// </summary>
        /// <param name="gpsPosition"></param>
        public void CheckFences(Vector2 gpsPosition)
        {


            foreach (var fence in fences)
            {
                bool positionInFence = OnlineMapsUtils.IsPointInPolygon(fence.Value.points, gpsPosition.x, gpsPosition.y);

                if (positionInFence)
                {
                    if (fencesContainingGPS.ContainsKey(fence.Key))
                    {
                        // Fence is detected and was previously detected, STAY -> OnGPSStayFence
                        onGPSStayFence.Invoke(fence);
                        print("Staying in fence " + fence.Key);
                    }
                    else
                    {
                        // Fence is detected but wasn't previously detected, ENTER -> OnGPSEnterFence
                        onGPSEnterFence.Invoke(fence);
                        fencesContainingGPS.Add(fence.Key, fence.Value);
                        print("Entering in fence " + fence.Key);
                    }
                }
                else
                {
                    if (fencesContainingGPS.ContainsKey(fence.Key))
                    {
                        // Fence isn't detected but was previously detected -> EXIT OnGPSExitFence
                        onGPSExitFence.Invoke(fence);
                        fencesContainingGPS.Remove(fence.Key);
                        print("Exiting fence " + fence.Key);
                    }
                    else
                    {
                        // Fence isn't detected and wasn't previously detected -> Do Nothing
                    }
                }
            }


        }
    }
}

