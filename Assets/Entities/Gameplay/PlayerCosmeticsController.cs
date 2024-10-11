using System;
using Entities.DataManagement.Cosmetics;
using Entities.GameManagement;
using Entities.Gameplay.Prefabs;
using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
    public class PlayerCosmeticsController : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _MeshRenderer;
        
        private StoreContentManager storeContentManager;
        
        private void Awake()
        {
            storeContentManager = GameManager.GetService<StoreContentManager>();
            if (PlayerPrefs.HasKey(ItemTypes.PlayerColor.ToString()))
            {
                _MeshRenderer.material.color = ColorMapper.ColorMap[PlayerPrefs.GetString(ItemTypes.PlayerColor.ToString())];
            }
            if (PlayerPrefs.HasKey(ItemTypes.Hat.ToString()))
            {
                if (storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.Hat.ToString()),
                        out var item))
                {
                    Instantiate(item.ItemWorldPrefab, _MeshRenderer.transform);
                }
            }
            if (PlayerPrefs.HasKey(ItemTypes.PlayerTexture.ToString()))
            {
                if (storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.PlayerTexture.ToString()),
                        out var item))
                {
                    _MeshRenderer.material.mainTexture = item.PlayerTexture.texture;
                }
            }
            if (PlayerPrefs.HasKey(ItemTypes.Hat.ToString()))
            {
                if (storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.ParticleEffect.ToString()),
                        out var item))
                {
                    Instantiate(item.ItemWorldPrefab, _MeshRenderer.transform);
                }
            }
        }
    }
}