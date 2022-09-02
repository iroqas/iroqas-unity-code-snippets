using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

/// <summary>
///  Iroqas - September 2021  
/// 
///     Checks if a device fits with the ProviderType required to use Vuforia AR.
/// 
/// -----------------------------------------------------------------------------------------------
/// 
/// This script offers some UnityEvent to let developers perform the required 
/// operations to enable and disable AR. 
/// 
/// When script is set active and enabled, it will check for the device's ProviderFusionType.
///     Some features availability requires a minimum ProviderFusionType.
///     For example, Ground Plane requires SENSOR_FUSION or SENSOR_FUSION_PLATFORM.
/// 
/// If the device fits with the minimum requirements, then onEnableARAvailable will be invoked.
///     This event should call to the required callbacks so Vuforia is setup to work as normal.
///     
/// Otherwise, if device is not capable to offer the required ProviderType, onEnableARNotAvailable 
/// will be invoked.
///     This event should call to the required callbacks so an alternative strategy for 
///     non-compatible devices is setup.
/// </summary>
/// 

namespace Iroqas.VuforiaUtils
{
    public class VuforiaARAvailability : MonoBehaviour
    {
        [Header("Minimum Provider Required")]


        public FusionProviderType minimumFusionProviderType;      // Minimum provider required.   


        [Header("Events")]

        public UnityEvent onEnableARAvailable;              // Event invoked when AR is available.
        public UnityEvent onEnableARNotAvailable;           // Event invoked when AR is NOT available.
        public UnityEvent onDisable;                        // Event invoked to reset dependent GameObjects state.

    
        private void Awake()                            
        {
            onDisable.Invoke();                             // Invoked to Normalize the initial states of event dependent objects.
        }


        void OnEnable()                                 
        {
            if (IsARAvailable())    // If AR Available -> Invokes AR available event                           
            {
                onEnableARAvailable.Invoke();              
            }
            else                    // If AR NOT Available -> Invokes AR NOT available event
            {
                onEnableARNotAvailable.Invoke();            
            }
        }

        void OnDisable()
        {   
            // Invokes onDisable event

            onDisable.Invoke();                             
            Debug.Log("VuforiaARAvailaibility: DISABLED");
        }

        public bool IsARAvailable()
        {
            // Gets Active Fusion Provider

            FusionProviderType fusionProviderType = VuforiaRuntimeUtilities.GetActiveFusionProvider();    

            Debug.Log("VuforiaARAvailaibility - Active Fusion Provider: " + fusionProviderType); 
        
        
            //Checks if found FusionProviderType fits the minimum FusionProviderType requirements 

            if(fusionProviderType >= minimumFusionProviderType)                                                 
            {
                Debug.Log("VuforiaARAvailaibility: AR Available");
                return true;
            }
            else
            {
                Debug.Log("VuforiaARAvailaibility: AR NOT Available");
                return false;
            }
        }

    }

}
