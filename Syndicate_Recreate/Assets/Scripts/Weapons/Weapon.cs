using UnityEngine;
using System.Collections;

abstract public class Weapon : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	protected float m_fAttackRange = 20.0f;
	[SerializeField]
	protected float m_fFireSpeed = 1.0f;
    [SerializeField]
    protected float m_fDefaultDamages = 2.0f;
    [SerializeField]
    protected Animation m_pShootAnimation;
    [SerializeField]
    protected GameObject m_pGun;
    #endregion

    #region Variables (private)

    protected GameObject m_pWielder;

	protected float m_fLastAttackTime = 0.0f;

    #endregion

    public GameObject Gun
    {
        get
        {
            return m_pGun;
        }
    }
    public Vector3 GunPosition
    {
        get
        {
            return m_pGun.transform.position;
        }
    }



    void Start()
	{
		m_pWielder = transform.parent.gameObject;
	}

	public virtual void Shoot(Transform pTarget)
	{
		if (Time.fixedTime - m_fLastAttackTime >= m_fFireSpeed)
		{
			RaycastHit Hit;

			if (Physics.Linecast(m_pGun.transform.position, pTarget.transform.position, out Hit, (Map.AllButGroundLayer & LivingBeing.AllButDeadsLayer), QueryTriggerInteraction.Ignore))
			{
				LivingBeing tShotThing = Hit.collider.gameObject.GetComponent<LivingBeing>();

				if (tShotThing)
					OnShoot(tShotThing);
			}

			m_fLastAttackTime = Time.fixedTime;
			m_pShootAnimation.Play();
		}
	}

	public abstract void OnShoot(LivingBeing pShotThing);

	public float AttackRange
	{
		get { return m_fAttackRange; }
	}

	public GameObject Icon
	{
		get { return WeaponsIcons.GetIcon(gameObject.name); }
	}
}
