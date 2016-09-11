using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeleportationBeam : MonoBehaviour
{

    public Valve.VR.EVRButtonId buttonId = Valve.VR.EVRButtonId.k_EButton_Axis0;

    public GameObject laserPrefab;
    public GameObject reticlePrefab;
    public Transform player;

    public Animator additiveDissolveAnim;
    public AnimationClip additiveDissolveClip;

    public float indexOfRefraction;
    public float wallThickness;

    private Reticle reticle;
    private Laser laser;

    public float range = 20f;

    public Color enabledColor = Color.white;
    public Color disabledColor = Color.red;

    private SteamVR_TrackedObject controller;

    private RaycastHit target;
    private bool canTeleport;

    // Use this for initialization
    void Start()
    {

        GameObject laserObj = (GameObject)Instantiate(laserPrefab);
        GameObject reticleObj = (GameObject)Instantiate(reticlePrefab);

        laserObj.transform.SetParent(player);
        reticleObj.transform.SetParent(player);

        reticle = reticleObj.GetComponent<Reticle>();
        laser = laserObj.GetComponent<Laser>();

        controller = GetComponent<SteamVR_TrackedObject>();
    }

    /*
    // Update is called once per frame
    void Update()
    {

        laser.gameObject.SetActive(false);
        reticle.gameObject.SetActive(false);

        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)controller.index);

        if (device.GetPress(buttonId))
        {

            canTeleport = false;

            laser.gameObject.SetActive(true);
            reticle.gameObject.SetActive(true);

            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            List<Vector3> waypoints = new List<Vector3>();
            waypoints.Add(transform.position);

            reticle.transform.position = ray.origin + ray.direction * range;

            if (Physics.Raycast(ray, out hit, range, 1, QueryTriggerInteraction.Ignore))
            {

                target = hit;
                canTeleport = hit.collider.CompareTag("Teleportable"); // true;

                reticle.transform.position = target.point;
                reticle.transform.up = target.normal;

            }

            waypoints.Add(reticle.transform.position);

            laser.SetWaypoints(waypoints.ToArray());

            Color color = canTeleport ? enabledColor : disabledColor;
            laser.SetColor(color);
            reticle.SetColor(color);
            reticle.ShowPlayArea(canTeleport);
        }

        if (device.GetPressUp(buttonId) && canTeleport)
        {
            StartCoroutine(Teleport());
        }
    }
    */

    // Update is called once per frame
    void Update()
    {

        SteamVR_Controller.Device device = SteamVR_Controller.Input((int)controller.index);

        if (device.GetPress(buttonId))
        {

            canTeleport = false;

            laser.gameObject.SetActive(true);
            reticle.gameObject.SetActive(true);

            List<Vector3> waypoints = new List<Vector3>();

            if (CanTeleport(ref waypoints, ref target))
            {
                canTeleport = true;
                reticle.transform.up = target.normal;
            }

            if (waypoints.Count > 0)
            {
                reticle.transform.position = waypoints[waypoints.Count - 1];
            }

            laser.SetWaypoints(waypoints.ToArray());
            Color color = canTeleport ? enabledColor : disabledColor;
            laser.SetColor(color);
            reticle.SetColor(color);
            reticle.ShowPlayArea(canTeleport);
        }
        else
        {
            laser.gameObject.SetActive(false);
            reticle.gameObject.SetActive(false);
        }

        if (device.GetPressUp(buttonId) && canTeleport)
        {
            StartCoroutine(Teleport());
        }
    }

    /// <summary>
    /// Can Teleport
    /// </summary>
    /// <param name="waypoints"></param>
    /// <param name="final"></param>
    /// <returns></returns>
    private bool CanTeleport(ref List<Vector3> waypoints, ref RaycastHit final) {

          Ray ray = new Ray(transform.position, transform.forward);
        waypoints.Add(transform.position);

          return CanTeleportHelper(ref waypoints, ref final, range, ray, false);
    }

    /// <summary>
    /// Can Teleport Helper
    /// </summary>
    /// <param name="waypoints"></param>
    /// <param name="final"></param>
    /// <param name="rangeRemaining"></param>
    /// <param name="ray"></param>
    /// <param name="success"></param>
    /// <returns></returns>
    private bool CanTeleportHelper(ref List<Vector3> waypoints, ref RaycastHit final, float rangeRemaining, Ray ray, bool success)
    {

        if (rangeRemaining <= 0 || success)
        {
            return success;
        }

        RaycastHit hit;

        Vector3 waypoint = ray.origin + ray.direction * rangeRemaining;

        if (Physics.Raycast(ray, out hit, rangeRemaining, 1, QueryTriggerInteraction.Ignore))
        {

            final = hit;
            waypoint = final.point;

            rangeRemaining -= (ray.origin - hit.point).magnitude;

            if (hit.collider.CompareTag("Teleportable"))
            {

                rangeRemaining = 0;
                success = true;

            }

            else if (hit.collider.CompareTag("Reflect"))
            {

                ray.origin = hit.point;
                ray.direction = Vector3.Reflect(ray.direction, hit.normal);
            }

            else if (hit.collider.CompareTag("Refract"))
            {

                ray.origin = hit.point + ray.direction * wallThickness;

                Refract refract =
                hit.collider.gameObject.GetComponent<Refract>();

                int numHits = 0;

                RaycastHit[] hitChecks = Physics.RaycastAll(ray);
                foreach (RaycastHit hitCheck in hitChecks)
                {
                    if (hitCheck.collider.gameObject.GetInstanceID().Equals(hit.collider.gameObject.GetInstanceID()))
                    {
                        ++numHits;
                    }
                }

                bool isInside = numHits % 2 == 0;

                float indexOfRefractionNext = isInside ? 1f : refract.indexOfRefraction;

                Vector3 s = ray.direction;
                Vector3 n = hit.normal;
                float m = indexOfRefraction / indexOfRefractionNext;
                Vector3 nXs = Vector3.Cross(n, s);

                ray.direction = m * Vector3.Cross(n, -nXs) - n * Mathf.Sqrt(Mathf.Abs(1 - Mathf.Pow(m, 2f) * Mathf.Pow(nXs.magnitude, 2f)));

                indexOfRefraction = indexOfRefractionNext;
            }

            else
            {

                rangeRemaining = 0;
            }
        }
        else
        {

            rangeRemaining = 0;
        }

        waypoints.Add(waypoint);

        return CanTeleportHelper(ref waypoints, ref final, rangeRemaining, ray, success);
    }

    /// <summary>
    /// Teleport
    /// </summary>
    /// <returns></returns>
    private IEnumerator Teleport()
    {

        additiveDissolveAnim.SetTrigger("Appear");

        yield return new WaitForSeconds(additiveDissolveClip.length);

        player.position = target.point;

        player.up = target.normal;
    }


}