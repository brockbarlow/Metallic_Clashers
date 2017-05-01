namespace Combat.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class EnemyHealthBar : Image, IComponent
    {
        [SerializeField]
        private Enemy m_Enemy;

        private Rigidbody2D m_Rigidbody2D;
        private Collider2D m_Collider2D;
        private ContactFilter2D m_ContactFilter2D;

        private Bounds m_MeshBounds;

        private Vector3 m_OffsetPosition;

        public Enemy enemy { get { return m_Enemy; } set { m_Enemy = value; } }

        protected override void Awake()
        {
            base.Awake();

            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Collider2D = GetComponent<Collider2D>();

            m_ContactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        }

        protected override void Start()
        {
            base.Start();

            m_Enemy.health.onTotalValueChanged.AddListener(UpdateFillAmount);
            m_Enemy.onDestroy.AddListener(EnemyDestroyed);

            UpdateFillAmount();

            var enemyMono = m_Enemy.GetComponent<EnemyMono>();
            var meshRenderer = enemyMono.GetComponentInChildren<SkinnedMeshRenderer>();
            if (meshRenderer == null)
                return;

            m_MeshBounds = meshRenderer.sharedMesh.bounds;

            OnGUI();
        }

        private void LateUpdate()
        {
            var raycastHit2Ds = new RaycastHit2D[5];
            var hitColliders = m_Collider2D.Cast(Vector2.zero, m_ContactFilter2D, raycastHit2Ds);
            for (var i = 0; i < hitColliders; ++i)
                m_OffsetPosition += (Vector3)raycastHit2Ds[i].normal;

            if (hitColliders == 0)
                m_OffsetPosition = Vector3.MoveTowards(m_OffsetPosition, Vector3.zero, 0.25f);

            m_OffsetPosition = Vector3.Scale(m_OffsetPosition, new Vector3(0f, 1f, 0f));
        }

        private void OnGUI()
        {
            var enemyMono = m_Enemy.GetComponent<EnemyMono>();

            var currentPosition =
                Camera.main.WorldToScreenPoint(
                    m_Enemy.GetComponent<EnemyMono>().transform.position
                    + new Vector3(
                        0f,
                        m_MeshBounds.max.y * enemyMono.transform.lossyScale.y
                        + enemyMono.transform.position.y,
                        0f));

            rectTransform.position =
                Vector3.Scale(currentPosition + m_OffsetPosition, new Vector3(1f, 1f, 0f));
        }

        private void UpdateFillAmount()
        {
            fillAmount = m_Enemy.health.totalValue / m_Enemy.health.value;
            color = Color.Lerp(Color.red, Color.green, fillAmount);
        }

        private void EnemyDestroyed()
        {
            Destroy(gameObject);
        }
    }
}
