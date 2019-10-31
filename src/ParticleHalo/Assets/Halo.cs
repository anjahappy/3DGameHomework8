using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleData {
  public float angle;                                       // 粒子初始角度
  public float radius;                                      // 当前粒子半径
  public float before;                                // 粒子收缩前半径
  public float after;                                // 粒子收缩后半径

  public ParticleData(float angle, float radius, float before, float after) {
    this.angle = angle;
    this.radius = radius;
    this.before = before;
    this.after = after;
  }
}
public class Halo : MonoBehaviour {

    public ParticleSystem particleSystem;
    public Camera camera;
    public int particleNum = 10000; // 粒子数目
    public float minRadius = 5.0f;  // 光环最小半径
    public float maxRadius = 10.0f; // 光环最大半径

    private ParticleSystem.Particle[] particles;
    private ParticleData[] particleDatas;
    private int speedLevel = 5; // 粒子旋转速度水平
    private float particleSpeed = 0.1f;  // 粒子旋转速度

    private Ray ray;
    private RaycastHit hit;

    // 收缩前粒子位置
   
    // 粒子缩放的速度
    private float shrinkSpeed = 2f;
    private bool isshrink = false;


	// Use this for initialization
	void Start () {
        particles = new ParticleSystem.Particle[particleNum];
        particleDatas = new ParticleData[particleNum];          //新建粒子数据数组
        particleSystem.maxParticles = particleNum;

        particleSystem.Emit(particleNum);
        particleSystem.GetParticles(particles);

        Ndistribution nd = new Ndistribution();

        // 每个粒子在初始化的时候都设定好收缩前和收缩后的粒子半径
        for (int i = 0; i < particleNum; i++)
        {
            float angle = UnityEngine.Random.Range(0.0f, 360.0f);
            float radius = (float)nd.getNormalDistribution((minRadius+maxRadius)*0.5f, 1);
            float before = radius;
            float after = 0.7f * radius;

            if (after < minRadius * 1.1f)
            {
                float midRadius = minRadius * 1.2f;
                after = UnityEngine.Random.Range(UnityEngine.Random.Range(minRadius, midRadius), (minRadius * 1.1f));
            }
            particleDatas[i] = new ParticleData(angle,radius,before,after);
        }
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < particleNum; i++)
        {
            if (isshrink)
            {
                // 开始收缩
                if(particleDatas[i].radius > particleDatas[i].after)
                {
                    particleDatas[i].radius -= shrinkSpeed * (particleDatas[i].radius / particleDatas[i].after) * Time.deltaTime;
                }
            }
            else
            {
                // 开始还原
                if (particleDatas[i].radius < particleDatas[i].before)
                {
                    particleDatas[i].radius += shrinkSpeed * (particleDatas[i].before / particleDatas[i].radius) * Time.deltaTime;
                }
                else if (particleDatas[i].radius > particleDatas[i].before)
                {
                    particleDatas[i].radius = particleDatas[i].before;
                }
            }

            // 通过奇偶控制粒子顺时针或逆时针旋转 
            if (i % 2 == 0)  
            {  
                // 逆时针
                particleDatas[i].angle += (i % speedLevel + 1) * particleSpeed;  
            }  
            else  
            {  
                // 顺时针
                particleDatas[i].angle -= (i % speedLevel + 1) * particleSpeed;  
            }  

            particleDatas[i].angle = particleDatas[i].angle % 360;
            // 转换为弧度制
            float rad = particleDatas[i].angle / 180 * Mathf.PI;  

            // 更新粒子坐标
            particles[i].position = new Vector3(particleDatas[i].radius * Mathf.Cos(rad), particleDatas[i].radius * Mathf.Sin(rad), 0f);  
        }  

        particleSystem.SetParticles(particles, particleNum);  
  
        ray = camera.ScreenPointToRay(Input.mousePosition);  
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "button") isshrink = true;  
        else isshrink = false;  
	}
}

class Ndistribution{
    System.Random rand = new System.Random();

    public double getNormalDistribution(double mean, double stdDev)
    {
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return randNormal;
    }
}