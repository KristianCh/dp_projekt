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
        
        [SerializeField]
        private Transform _RootTransform;
        
        private StoreContentManager _storeContentManager;
        
        private void Awake()
        {
            _storeContentManager = GameManager.GetService<StoreContentManager>();
            if (ItemTypes.PlayerTexture.ToString() == "NoTexture") 
                _MeshRenderer.material.color = ColorMapper.ColorMap[PlayerPrefs.GetString(ItemTypes.PlayerColor.ToString())];
            
            if (_storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.PlayerTexture.ToString()),
                    out var texture)) _MeshRenderer.material.mainTexture = texture.PlayerTexture.texture;
            
            if (_storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.Hat.ToString()),
                    out var hat)) Instantiate(hat.ItemWorldPrefab, _RootTransform.transform);
            
            if (_storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.ParticleEffect.ToString()),
                    out var particle)) Instantiate(particle.ItemWorldPrefab, _RootTransform.transform);
        }
    }
}