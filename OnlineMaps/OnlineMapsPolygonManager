using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
///  Iroqas - September 2021  
///  
///     Polygon handler for OnlineMaps Unity's asset.
/// -----------------------------------------------------------------------------------------------
/// 
/// >> NOTE: Requires OnlineMaps asset.
/// 
/// This scripts brings a key-value handling system to manage OnlineMaps polygons. 
/// 
/// </summary>
/// 

namespace Iroqas.OnlineMapsUtilities
{


    public class OnlineMapsPolygonManager : MonoBehaviour
    {
        protected bool isDirty;


        // Dictionaries
        protected Dictionary<string, OnlineMapsDrawingPoly> polygons = new Dictionary<string, OnlineMapsDrawingPoly>();
        protected Dictionary<string, OnlineMapsDrawingPoly> visiblePolygons = new Dictionary<string, OnlineMapsDrawingPoly>();

        // Events
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onAddPolygon = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onUpdatePolygon = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onRemovePolygon = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onPolygonSetVisible = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();
        protected UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>> onPolygonSetInvisible = new UnityEvent<KeyValuePair<string, OnlineMapsDrawingPoly>>();

        private void Start()
        {

            onPolygonSetVisible.AddListener(LoadPolygonToOnlineMapsCallback);
            onPolygonSetInvisible.AddListener(UnloadPolygonFromOnlineMapsCallback);

        
        }

    

        private void LateUpdate()
        {
            if (isDirty)
            {
                isDirty = false;
                OnlineMaps.instance.needRedraw = true;
                print("was dirty");
            }
        }



        #region Event handlers management.
        /// <summary>
        /// Adds listener to onAddPolygon Event.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="getAllExistingElements">When added, event will call this new listener for each existing polygon.</param>
        public void AddOnAddPolygonListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback, bool getAllExistingElements = false)
        {
            if (getAllExistingElements)
            {
                foreach(var polygon in polygons)
                {
                    callback.Invoke(polygon);
                }

                onAddPolygon.AddListener(callback);

            }

        }


        /// <summary>
        /// Remove listener to onAddPolygon Event.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnAddPolygonListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onAddPolygon.RemoveListener(callback);
        }

        /// <summary>
        /// Adds listener to onRemovePolygon Event.
        /// </summary>
        /// <param name="callback"></param>
        public void AddOnRemovePolygonListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onRemovePolygon.AddListener(callback);
        }


        /// <summary>
        /// Removes listener from onRemovePolygon Event.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnRemovePolygonListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onRemovePolygon.RemoveListener(callback);
        }


        /// <summary>
        /// Adds listener to onPolygonSetVisible Event.
        /// </summary>
        /// <param name="callback"></param>
        public void AddOnPolygonSetVisibleListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback, bool getAllExistingElements = false)
        {
            if (getAllExistingElements)
            {
                foreach (var polygon in visiblePolygons)
                {
                    callback.Invoke(polygon);
                }

                onPolygonSetVisible.AddListener(callback);

            }
        }


        /// <summary>
        /// Removes listener from onPolygonSetVisible Event.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnPolygonSetVisibleListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onPolygonSetVisible.RemoveListener(callback);
        }

        /// <summary>
        /// Adds listener to onPolygonSetInvisible Event.
        /// </summary>
        /// <param name="callback"></param>
        public void AddOnSetInvisiblePolygonListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onPolygonSetInvisible.AddListener(callback);
        }


        /// <summary>
        /// Removes listener to onPolygonSetInvisible Event.
        /// </summary>
        /// <param name="callback"></param>
        public void RemoveOnPolygonSetInvisibleListener(UnityAction<KeyValuePair<string, OnlineMapsDrawingPoly>> callback)
        {
            onPolygonSetInvisible.RemoveListener(callback);
        }

        /// <summary>
        /// Handler to add polygon to OnlineMaps when it's set to visible.
        /// </summary>
        /// <param name="data"></param>
        private void LoadPolygonToOnlineMapsCallback(KeyValuePair<string, OnlineMapsDrawingPoly> data)
        {
            print(data.Key + " - Loaded to OnlineMaps");
            OnlineMapsDrawingElementManager.AddItem(data.Value);
            isDirty = true;
        }

        /// <summary>
        /// Handler to remove polygon to OnlineMaps when it's set to invisible.
        /// </summary>
        /// <param name="data"></param>
        private void UnloadPolygonFromOnlineMapsCallback(KeyValuePair<string, OnlineMapsDrawingPoly> data)
        {
            print(data.Key + " - Unloaded from OnlineMaps");
            OnlineMapsDrawingElementManager.RemoveItem(data.Value, false);
            isDirty = true;
        }

        #endregion

        #region Polygon ADD/UPDATE/REMOVE
        /// <summary>
        /// Adds a polygon to the manager. To show it, it's required to call SetPolygonVisibility().
        /// </summary>
        /// <param name="key"></param>
        /// <param name="polygon"></param>
        public void AddPolygon(string key, OnlineMapsDrawingPoly polygon)
        {
            if (polygons.ContainsKey(key))
            {
                throw new System.Exception("Can not add a polygon. Key already exist.");
            }

            polygons.Add(key, polygon);

            onAddPolygon.Invoke(new KeyValuePair<string, OnlineMapsDrawingPoly>(key, polygons[key]));
        }


        /// <summary>
        /// Updates the reference of a polygon in the manager.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="polygon">New reference</param>
        public void UpdatePolygon(string key, OnlineMapsDrawingPoly polygon)
        {
            polygons[key] = polygon; 

            onUpdatePolygon.Invoke(new KeyValuePair<string, OnlineMapsDrawingPoly>(key, polygons[key]));
        }


        /// <summary>
        /// Removes a polygon from the manager.
        /// </summary>
        /// <param name="key"></param>
        public void RemovePolygon(string key)
        {
            SetPolygonVisibility(key, false);

            polygons.Remove(key);

            onRemovePolygon.Invoke(new KeyValuePair<string, OnlineMapsDrawingPoly>(key, polygons[key]));
        }

        #endregion

        #region Other Methods
        /// <summary>
        /// Checks if a polygon exist in this manager.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool PolygonExist(string key)
        {
            return polygons.ContainsKey(key);
        }


        /// <summary>
        /// Checks if a polygon is visible or not.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsPolygonVisible(string key)
        {
            return visiblePolygons.ContainsKey(key);
        }




        /// <summary>
        /// Returns a reference from a polygon.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public OnlineMapsDrawingPoly GetPolygon(string key)
        {
           OnlineMapsDrawingPoly result = null;

           polygons.TryGetValue(key, out result);

           return result;
        }


        /// <summary>
        /// Sets the visiblity of a polygon.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="visible"></param>
        public void SetPolygonVisibility(string key, bool visible)
        {

            if (visible)
            {
                if (IsPolygonVisible(key))
                {
                    return;
                }

                print(key + " - Added to showing");
                visiblePolygons.Add(key, polygons[key]);

                onPolygonSetVisible.Invoke(new KeyValuePair<string, OnlineMapsDrawingPoly>(key, polygons[key]));
            }
            else
            {
                if (!IsPolygonVisible(key))
                {
                    return;
                }

                print(key + " - Removed from showing");

                visiblePolygons.Remove(key);

                onPolygonSetInvisible.Invoke(new KeyValuePair<string, OnlineMapsDrawingPoly>(key, polygons[key]));
            }
        

        }

        #endregion

        /// <summary>
        /// Some utilities for future implementations.
        /// </summary>
        public static class Util
        {
            /// <summary>
            /// Checks if a given position (Lon, Lat) is inside a polygon.
            /// </summary>
            /// <param name="position"></param>
            /// <param name="polygon"></param>
            /// <returns></returns>
            public static bool IsPositionInPolygon(Vector2 position, IEnumerable<Vector2> polygon)
            {
                return OnlineMapsUtils.IsPointInPolygon(new List<Vector2>(polygon), position.x, position.y);
            }

            /// <summary>
            /// Calculates the distance between two positions (Lon, Lat)
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public static double Distance(Vector2 from, Vector2 to)
            {
                return OnlineMapsUtils.DistanceBetweenPointsD(from, to);
            }

            /// <summary>
            /// Factory method to create polygons.
            /// </summary>
            /// <param name="polygon"></param>
            /// <param name="borderColor"></param>
            /// <param name="borderWidth"></param>
            /// <param name="backgroundColor"></param>
            /// <returns></returns>
            public static OnlineMapsDrawingPoly CreatePolygon(IEnumerable<Vector2> polygon, Color borderColor, float borderWidth, Color backgroundColor)
            {
                return new OnlineMapsDrawingPoly(polygon, borderColor, borderWidth, backgroundColor);
            }
        }


    }

}
