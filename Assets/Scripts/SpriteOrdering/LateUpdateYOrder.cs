using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOTU {
    [RequireComponent(typeof(SpriteRenderer))]
    public class LateUpdateYOrder : StaticYOrder
    {
        void LateUpdate() {
            UpdateSortingOrder();
        }
    }
}