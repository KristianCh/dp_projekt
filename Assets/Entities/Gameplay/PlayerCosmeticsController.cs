using System;
using Entities.DataManagement;
using Entities.DataManagement.Cosmetics;
using Entities.GameManagement;
using Entities.Utils;
using UnityEngine;

namespace Entities.Gameplay
{
    /// <summary>
    /// Controller applying cosmetics to player object in game.
    /// </summary>
    public class PlayerCosmeticsController : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _MeshRenderer;
        
        [SerializeField]
        private Transform _RootTransform;
        
        private StoreContentManager _storeContentManager;
        
        /// <summary>
        /// Sets player cosmetics.
        /// </summary>
        private void Awake()
        {
            _storeContentManager = GameManager.GetService<StoreContentManager>();
            if (PlayerPrefs.GetString(ItemTypes.PlayerTexture.ToString()) == "NoTexture") 
                _MeshRenderer.material.color = ColorMapper.ColorMap[PlayerPrefs.GetString(ItemTypes.PlayerColor.ToString())];
            else if (_storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.PlayerTexture.ToString()),
                    out var texture)) _MeshRenderer.material.mainTexture = texture.PlayerTexture.texture;
            
            if (_storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.Hat.ToString()),
                    out var hat)) Instantiate(hat.ItemWorldPrefab, _RootTransform.transform);
            
            if (_storeContentManager.TryGetItemByItemCode(PlayerPrefs.GetString(ItemTypes.ParticleEffect.ToString()),
                    out var particle)) Instantiate(particle.ItemWorldPrefab, _RootTransform.transform);
        }
    }
}