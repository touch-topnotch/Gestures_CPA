using UnityEngine;

// Script that passes the distance traveled by the Trail Renderer's Head when it moves to the material's scroll UV
// The Texture Mode of the Trail Renderer must be Tile
// The value passed to the material is between 0 and 1.
//[ExecuteAlways]
public class MoveToTrailUV : MonoBehaviour
{
    [System.Serializable]
    public struct MaterialData
    {
        public MaterialData(TrailRenderer trailRenderer, Vector2 uvScale, float move)
        {
            m_trailRenderer = trailRenderer;
            m_originalTime = m_trailRenderer.time;
            m_uvTiling = uvScale;
            m_move = move;
        }
        
        public TrailRenderer m_trailRenderer;
        public float m_originalTime;
        public Vector2 m_uvTiling;
        public float m_move;
    }

#if UNITY_EDITOR
    //public bool m_overrideMaterial = true;
#endif
    public Transform m_moveObject;
    public string m_shaderPropertyName = "_MoveToMaterialUV"; // Property name that will receive UV values in the shader.
    public int m_shaderPropertyID; // ID for not using strings in shader properties
    public MaterialData[] m_materialData;

    public float fadeOutDuration = 0.5f;  // Duration for the trail to fade out after the hit

    private bool isAttacking = true;
    private float originalTrailTime;
    private float fadeOutTimer = 0f;
    
    private Vector3 m_beforePosW = Vector3.zero;
    void Start()
    {
        Initialize();
    }

    void LateUpdate()
    {
        if (m_moveObject == null)
            return;
        if (m_materialData == null || m_materialData.Length == 0)
            return;

        Vector3 nowPosW = m_moveObject.transform.position;
        if (nowPosW == m_beforePosW)
            return; // If there is no change in position, do nothing

        float distance = Vector3.Distance(nowPosW, m_beforePosW);
        m_beforePosW = nowPosW;

        for (int i = 0; i < m_materialData.Length; i++)
        {
            if (m_materialData[i].m_trailRenderer == null)
                continue;

            m_materialData[i].m_move += distance * m_materialData[i].m_uvTiling.x;
            // To prevent the m_move value from becoming too large, only the remaining values ​​are passed if they are greater than 1. (They must already be multiplied by m_uvTiling.x)
            if (m_materialData[i].m_move > 1f)
            {
                m_materialData[i].m_move = m_materialData[i].m_move % 1f;
            }

            // Record without checking for property existence. There is a problem that the material version is continuously processed as changed if the property exists.
            TrailRenderer trailRenderer = m_materialData[i].m_trailRenderer;
            if (trailRenderer != null)
            {
                Material mat = trailRenderer.sharedMaterial;
                if (mat != null)
                {
                    mat.SetFloat(m_shaderPropertyID, m_materialData[i].m_move);
                }
            }
        }
    }

    public void Initialize()
    {
        if (m_materialData == null || m_materialData.Length == 0)
            return;
        
        m_shaderPropertyID = Shader.PropertyToID(m_shaderPropertyName);

        for (int i = 0; i < m_materialData.Length; i++)
        {
            m_materialData[i].m_move = 0f;
            TrailRenderer trailRenderer = m_materialData[i].m_trailRenderer;
            if (trailRenderer != null)
            {
                Material mat = trailRenderer.sharedMaterial;
                if (mat != null)
                {
                    m_materialData[i].m_uvTiling = mat.mainTextureScale;
                }
            }
        }
    }
 
    private void Update()
    {
        
        if (!isAttacking)
        {
            fadeOutTimer += Time.deltaTime;
            foreach (var materialData in m_materialData)
            {
                if (materialData.m_trailRenderer.time > 0)
                    materialData.m_trailRenderer.time = Mathf.Lerp(originalTrailTime, 0, fadeOutTimer / fadeOutDuration);
            }

            //trailRenderer.time = Mathf.Lerp(originalTrailTime, 0, fadeOutTimer / fadeOutDuration);
            
        }
    }
 
    public void PlayTrails()
    {
        isAttacking = true;
        fadeOutTimer = 0f;
        foreach (var materialData in m_materialData)
        {
            materialData.m_trailRenderer.Clear();
            materialData.m_trailRenderer.emitting = true;
            materialData.m_trailRenderer.time = materialData.m_originalTime;
            
            
        }
    }
    
    public void StopTrails()
    {
        isAttacking = false;
        foreach (var materialData in m_materialData)
        {
            materialData.m_trailRenderer.emitting = false;
        }
    }
}
