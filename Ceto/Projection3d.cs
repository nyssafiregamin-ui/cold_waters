using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000091 RID: 145
	public class Projection3d : IProjection
	{
		// Token: 0x060003E6 RID: 998 RVA: 0x00016AE4 File Offset: 0x00014CE4
		public Projection3d(Ocean ocean)
		{
			this.m_ocean = ocean;
			this.m_projectorP = new double[16];
			this.m_projectorV = new double[16];
			this.m_projectorVP = new double[16];
			this.m_projectorIVP = new double[16];
			this.m_projectorR = new double[16];
			this.m_projectorI = new double[16];
			this.MATRIX_BUFFER0 = new double[16];
			this.MATRIX_BUFFER1 = new double[16];
			this.VECTOR_BUFFER = new double[4];
			double[] array = new double[3];
			array[1] = 1.0;
			this.m_up = array;
			this.m_xaxis = new double[3];
			this.m_yaxis = new double[3];
			this.m_zaxis = new double[3];
			this.m_a = new double[4];
			this.m_b = new double[4];
			this.m_ab = new double[4];
			this.m_pos = new double[3];
			this.m_dir = new double[3];
			this.m_lookAt = new double[3];
			this.m_p = new double[4];
			this.m_q = new double[4];
			this.m_p0 = new double[4];
			this.m_p1 = new double[4];
			this.Identity(this.m_projectorR);
			this.m_pointList = new List<double[]>(32);
			for (int i = 0; i < 32; i++)
			{
				this.m_pointList.Add(new double[3]);
			}
			this.m_frustumCorners = new List<double[]>(8);
			for (int j = 0; j < 8; j++)
			{
				this.m_frustumCorners.Add(new double[3]);
			}
			this.m_quad = new List<double[]>(4);
			this.m_quad.Add(new double[]
			{
				0.0,
				0.0,
				0.0,
				1.0
			});
			this.m_quad.Add(new double[]
			{
				1.0,
				0.0,
				0.0,
				1.0
			});
			this.m_quad.Add(new double[]
			{
				1.0,
				1.0,
				0.0,
				1.0
			});
			this.m_quad.Add(new double[]
			{
				0.0,
				1.0,
				0.0,
				1.0
			});
			this.m_corners = new List<double[]>(8);
			this.m_corners.Add(new double[]
			{
				-1.0,
				-1.0,
				-1.0,
				1.0
			});
			this.m_corners.Add(new double[]
			{
				1.0,
				-1.0,
				-1.0,
				1.0
			});
			this.m_corners.Add(new double[]
			{
				1.0,
				1.0,
				-1.0,
				1.0
			});
			this.m_corners.Add(new double[]
			{
				-1.0,
				1.0,
				-1.0,
				1.0
			});
			this.m_corners.Add(new double[]
			{
				-1.0,
				-1.0,
				1.0,
				1.0
			});
			this.m_corners.Add(new double[]
			{
				1.0,
				-1.0,
				1.0,
				1.0
			});
			this.m_corners.Add(new double[]
			{
				1.0,
				1.0,
				1.0,
				1.0
			});
			this.m_corners.Add(new double[]
			{
				-1.0,
				1.0,
				1.0,
				1.0
			});
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060003E8 RID: 1000 RVA: 0x00016E4C File Offset: 0x0001504C
		public bool IsDouble
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00016E50 File Offset: 0x00015050
		public void UpdateProjection(Camera cam, CameraData data, bool projectSceneView)
		{
			if (cam == null || data == null)
			{
				return;
			}
			if (data.projection == null)
			{
				data.projection = new ProjectionData();
			}
			if (data.projection.updated)
			{
				return;
			}
			if (!projectSceneView && cam.name == "SceneCamera" && Camera.main != null)
			{
				cam = Camera.main;
			}
			this.AimProjector(cam);
			this.MulMatrixByMatrix(this.m_projectorVP, this.m_projectorP, this.m_projectorV);
			this.CreateRangeMatrix(cam, this.m_projectorVP);
			this.Inverse(this.MATRIX_BUFFER0, this.m_projectorVP);
			this.MulMatrixByMatrix(this.m_projectorIVP, this.MATRIX_BUFFER0, this.m_projectorR);
			this.HProject(this.m_projectorIVP, this.m_quad[0], this.VECTOR_BUFFER);
			this.SetRow(0, this.m_projectorI, this.VECTOR_BUFFER);
			this.HProject(this.m_projectorIVP, this.m_quad[1], this.VECTOR_BUFFER);
			this.SetRow(1, this.m_projectorI, this.VECTOR_BUFFER);
			this.HProject(this.m_projectorIVP, this.m_quad[2], this.VECTOR_BUFFER);
			this.SetRow(2, this.m_projectorI, this.VECTOR_BUFFER);
			this.HProject(this.m_projectorIVP, this.m_quad[3], this.VECTOR_BUFFER);
			this.SetRow(3, this.m_projectorI, this.VECTOR_BUFFER);
			this.Inverse(this.MATRIX_BUFFER0, this.m_projectorR);
			this.MulMatrixByMatrix(this.MATRIX_BUFFER1, this.MATRIX_BUFFER0, this.m_projectorVP);
			this.CopyMatrix(ref data.projection.projectorVP, this.MATRIX_BUFFER1);
			this.CopyMatrix(ref data.projection.interpolation, this.m_projectorI);
			data.projection.updated = true;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00017048 File Offset: 0x00015248
		private void AimProjector(Camera cam)
		{
			this.CopyMatrix(this.m_projectorP, cam.projectionMatrix);
			this.CopyVector3(this.m_pos, cam.transform.position);
			this.CopyVector3(this.m_dir, cam.transform.forward);
			double num = (double)this.m_ocean.level;
			double num2 = Math.Max(1.0, (double)this.m_ocean.FindMaxDisplacement(true));
			double num3 = Math.Max(0.0, num2 + 10.0);
			if (this.m_pos[1] < num)
			{
				this.m_pos[1] = num;
			}
			this.m_pos[1] += num3;
			this.m_lookAt[0] = this.m_pos[0] + this.m_dir[0] * 50.0;
			this.m_lookAt[1] = (double)this.m_ocean.level;
			this.m_lookAt[2] = this.m_pos[2] + this.m_dir[2] * 50.0;
			this.LookAt(this.m_pos, this.m_lookAt, this.m_up);
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00017174 File Offset: 0x00015374
		private void CreateRangeMatrix(Camera cam, double[] projectorVP)
		{
			this.Identity(this.m_projectorR);
			this.CopyMatrix(this.MATRIX_BUFFER0, cam.worldToCameraMatrix);
			this.MulMatrixByMatrix(this.MATRIX_BUFFER1, this.m_projectorP, this.MATRIX_BUFFER0);
			this.Inverse(this.MATRIX_BUFFER0, this.MATRIX_BUFFER1);
			double num = (double)this.m_ocean.level;
			double num2 = Math.Max(1.0, (double)this.m_ocean.FindMaxDisplacement(true));
			for (int i = 0; i < 8; i++)
			{
				this.MulVector4ByMatrix(this.m_p, this.m_corners[i], this.MATRIX_BUFFER0);
				this.m_frustumCorners[i][0] = this.m_p[0] / this.m_p[3];
				this.m_frustumCorners[i][1] = this.m_p[1] / this.m_p[3];
				this.m_frustumCorners[i][2] = this.m_p[2] / this.m_p[3];
			}
			int num3 = 0;
			for (int j = 0; j < 8; j++)
			{
				if (this.m_frustumCorners[j][1] <= num + num2 && this.m_frustumCorners[j][1] >= num - num2)
				{
					this.m_pointList[num3][0] = this.m_frustumCorners[j][0];
					this.m_pointList[num3][1] = this.m_frustumCorners[j][1];
					this.m_pointList[num3][2] = this.m_frustumCorners[j][2];
					num3++;
				}
			}
			for (int k = 0; k < 12; k++)
			{
				int index = Projection3d.m_indices[k, 0];
				this.m_p0[0] = this.m_frustumCorners[index][0];
				this.m_p0[1] = this.m_frustumCorners[index][1];
				this.m_p0[2] = this.m_frustumCorners[index][2];
				int index2 = Projection3d.m_indices[k, 1];
				this.m_p1[0] = this.m_frustumCorners[index2][0];
				this.m_p1[1] = this.m_frustumCorners[index2][1];
				this.m_p1[2] = this.m_frustumCorners[index2][2];
				if (this.SegmentPlaneIntersection(this.m_p0, this.m_p1, this.m_up, num + num2, this.m_pointList[num3]))
				{
					num3++;
				}
				if (this.SegmentPlaneIntersection(this.m_p0, this.m_p1, this.m_up, num - num2, this.m_pointList[num3]))
				{
					num3++;
				}
			}
			if (num3 > this.m_pointList.Count)
			{
				throw new InvalidOperationException("Count can not be greater than poin list count");
			}
			if (num3 == 0)
			{
				this.m_projectorR[0] = 1.0;
				this.m_projectorR[12] = 0.0;
				this.m_projectorR[5] = 1.0;
				this.m_projectorR[13] = 0.0;
				return;
			}
			double num4 = double.PositiveInfinity;
			double num5 = double.PositiveInfinity;
			double num6 = double.NegativeInfinity;
			double num7 = double.NegativeInfinity;
			for (int l = 0; l < num3; l++)
			{
				this.m_q[0] = this.m_pointList[l][0];
				this.m_q[1] = num;
				this.m_q[2] = this.m_pointList[l][2];
				this.m_q[3] = 1.0;
				this.MulVector4ByMatrix(this.m_p, this.m_q, projectorVP);
				this.m_p[0] /= this.m_p[3];
				this.m_p[1] /= this.m_p[3];
				if (this.m_p[0] < num4)
				{
					num4 = this.m_p[0];
				}
				if (this.m_p[1] < num5)
				{
					num5 = this.m_p[1];
				}
				if (this.m_p[0] > num6)
				{
					num6 = this.m_p[0];
				}
				if (this.m_p[1] > num7)
				{
					num7 = this.m_p[1];
				}
			}
			this.m_projectorR[0] = num6 - num4;
			this.m_projectorR[12] = num4;
			this.m_projectorR[5] = num7 - num5;
			this.m_projectorR[13] = num5;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0001760C File Offset: 0x0001580C
		private void HProject(double[] ivp, double[] corner, double[] result)
		{
			corner[2] = -1.0;
			this.MulVector4ByMatrix(this.m_a, corner, ivp);
			corner[2] = 1.0;
			this.MulVector4ByMatrix(this.m_b, corner, ivp);
			double num = (double)this.m_ocean.level;
			this.Sub4(this.m_ab, this.m_b, this.m_a);
			double num2 = (this.m_a[3] * num - this.m_a[1]) / (this.m_ab[1] - this.m_ab[3] * num);
			result[0] = this.m_a[0] + this.m_ab[0] * num2;
			result[1] = this.m_a[1] + this.m_ab[1] * num2;
			result[2] = this.m_a[2] + this.m_ab[2] * num2;
			result[3] = this.m_a[3] + this.m_ab[3] * num2;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x000176F4 File Offset: 0x000158F4
		private bool SegmentPlaneIntersection(double[] a, double[] b, double[] n, double d, double[] q)
		{
			this.Sub3(this.m_ab, b, a);
			double num = (d - this.Dot3(n, a)) / this.Dot3(n, this.m_ab);
			if (num > --0.0 && num <= 1.0)
			{
				q[0] = a[0] + num * this.m_ab[0];
				q[1] = a[1] + num * this.m_ab[1];
				q[2] = a[2] + num * this.m_ab[2];
				return true;
			}
			return false;
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00017780 File Offset: 0x00015980
		public void LookAt(double[] position, double[] target, double[] up)
		{
			this.Sub3(this.m_zaxis, position, target);
			this.Normalize3(this.m_zaxis);
			this.Cross3(this.m_xaxis, up, this.m_zaxis);
			this.Normalize3(this.m_xaxis);
			this.Cross3(this.m_yaxis, this.m_zaxis, this.m_xaxis);
			this.m_projectorV[0] = this.m_xaxis[0];
			this.m_projectorV[4] = this.m_xaxis[1];
			this.m_projectorV[8] = this.m_xaxis[2];
			this.m_projectorV[12] = -this.Dot3(this.m_xaxis, position);
			this.m_projectorV[1] = this.m_yaxis[0];
			this.m_projectorV[5] = this.m_yaxis[1];
			this.m_projectorV[9] = this.m_yaxis[2];
			this.m_projectorV[13] = -this.Dot3(this.m_yaxis, position);
			this.m_projectorV[2] = this.m_zaxis[0];
			this.m_projectorV[6] = this.m_zaxis[1];
			this.m_projectorV[10] = this.m_zaxis[2];
			this.m_projectorV[14] = -this.Dot3(this.m_zaxis, position);
			this.m_projectorV[3] = 0.0;
			this.m_projectorV[7] = 0.0;
			this.m_projectorV[11] = 0.0;
			this.m_projectorV[15] = 1.0;
			this.m_projectorV[0] *= -1.0;
			this.m_projectorV[4] *= -1.0;
			this.m_projectorV[8] *= -1.0;
			this.m_projectorV[12] *= -1.0;
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00017960 File Offset: 0x00015B60
		private void MulVector4ByMatrix(double[] des, double[] src, double[] m)
		{
			des[0] = m[0] * src[0] + m[4] * src[1] + m[8] * src[2] + m[12] * src[3];
			des[1] = m[1] * src[0] + m[5] * src[1] + m[9] * src[2] + m[13] * src[3];
			des[2] = m[2] * src[0] + m[6] * src[1] + m[10] * src[2] + m[14] * src[3];
			des[3] = m[3] * src[0] + m[7] * src[1] + m[11] * src[2] + m[15] * src[3];
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x000179FC File Offset: 0x00015BFC
		private void MulMatrixByMatrix(double[] des, double[] m1, double[] m2)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					des[i + j * 4] = m1[i] * m2[0 + j * 4] + m1[i + 4] * m2[1 + j * 4] + m1[i + 8] * m2[2 + j * 4] + m1[i + 12] * m2[3 + j * 4];
				}
			}
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00017A6C File Offset: 0x00015C6C
		private void Identity(double[] m)
		{
			m[0] = 1.0;
			m[4] = 0.0;
			m[8] = 0.0;
			m[12] = 0.0;
			m[1] = 0.0;
			m[5] = 1.0;
			m[9] = 0.0;
			m[13] = 0.0;
			m[2] = 0.0;
			m[6] = 0.0;
			m[10] = 1.0;
			m[14] = 0.0;
			m[3] = 0.0;
			m[7] = 0.0;
			m[11] = 0.0;
			m[15] = 1.0;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00017B40 File Offset: 0x00015D40
		private void CopyMatrix(double[] des, Matrix4x4 m)
		{
			des[0] = (double)m.m00;
			des[4] = (double)m.m01;
			des[8] = (double)m.m02;
			des[12] = (double)m.m03;
			des[1] = (double)m.m10;
			des[5] = (double)m.m11;
			des[9] = (double)m.m12;
			des[13] = (double)m.m13;
			des[2] = (double)m.m20;
			des[6] = (double)m.m21;
			des[10] = (double)m.m22;
			des[14] = (double)m.m23;
			des[3] = (double)m.m30;
			des[7] = (double)m.m31;
			des[11] = (double)m.m32;
			des[15] = (double)m.m33;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00017C04 File Offset: 0x00015E04
		private void CopyMatrix(ref Matrix4x4 des, double[] m)
		{
			des.m00 = (float)m[0];
			des.m01 = (float)m[4];
			des.m02 = (float)m[8];
			des.m03 = (float)m[12];
			des.m10 = (float)m[1];
			des.m11 = (float)m[5];
			des.m12 = (float)m[9];
			des.m13 = (float)m[13];
			des.m20 = (float)m[2];
			des.m21 = (float)m[6];
			des.m22 = (float)m[10];
			des.m23 = (float)m[14];
			des.m30 = (float)m[3];
			des.m31 = (float)m[7];
			des.m32 = (float)m[11];
			des.m33 = (float)m[15];
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00017CB8 File Offset: 0x00015EB8
		private void CopyVector3(double[] des, Vector3 v)
		{
			des[0] = (double)v.x;
			des[1] = (double)v.y;
			des[2] = (double)v.z;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x00017CDC File Offset: 0x00015EDC
		private void Normalize3(double[] v)
		{
			double num = 1.0 / Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
			v[0] *= num;
			v[1] *= num;
			v[2] *= num;
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00017D34 File Offset: 0x00015F34
		private void Sub3(double[] des, double[] v1, double[] v2)
		{
			des[0] = v1[0] - v2[0];
			des[1] = v1[1] - v2[1];
			des[2] = v1[2] - v2[2];
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x00017D54 File Offset: 0x00015F54
		private void Sub4(double[] des, double[] v1, double[] v2)
		{
			des[0] = v1[0] - v2[0];
			des[1] = v1[1] - v2[1];
			des[2] = v1[2] - v2[2];
			des[3] = v1[3] - v2[3];
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00017D8C File Offset: 0x00015F8C
		private double Dot3(double[] v1, double[] v2)
		{
			return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00017DA8 File Offset: 0x00015FA8
		private void Cross3(double[] des, double[] v1, double[] v2)
		{
			des[0] = v1[1] * v2[2] - v1[2] * v2[1];
			des[1] = v1[2] * v2[0] - v1[0] * v2[2];
			des[2] = v1[0] * v2[1] - v1[1] * v2[0];
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00017DEC File Offset: 0x00015FEC
		private double Minor(double[] m, int r0, int r1, int r2, int c0, int c1, int c2)
		{
			return m[r0 + c0 * 4] * (m[r1 + c1 * 4] * m[r2 + c2 * 4] - m[r2 + c1 * 4] * m[r1 + c2 * 4]) - m[r0 + c1 * 4] * (m[r1 + c0 * 4] * m[r2 + c2 * 4] - m[r2 + c0 * 4] * m[r1 + c2 * 4]) + m[r0 + c2 * 4] * (m[r1 + c0 * 4] * m[r2 + c1 * 4] - m[r2 + c0 * 4] * m[r1 + c1 * 4]);
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00017E88 File Offset: 0x00016088
		private double Determinant(double[] m)
		{
			return m[0] * this.Minor(m, 1, 2, 3, 1, 2, 3) - m[4] * this.Minor(m, 1, 2, 3, 0, 2, 3) + m[8] * this.Minor(m, 1, 2, 3, 0, 1, 3) - m[12] * this.Minor(m, 1, 2, 3, 0, 1, 2);
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x00017EE0 File Offset: 0x000160E0
		private void Adjoint(double[] des, double[] m)
		{
			des[0] = this.Minor(m, 1, 2, 3, 1, 2, 3);
			des[4] = -this.Minor(m, 0, 2, 3, 1, 2, 3);
			des[8] = this.Minor(m, 0, 1, 3, 1, 2, 3);
			des[12] = -this.Minor(m, 0, 1, 2, 1, 2, 3);
			des[1] = -this.Minor(m, 1, 2, 3, 0, 2, 3);
			des[5] = this.Minor(m, 0, 2, 3, 0, 2, 3);
			des[9] = -this.Minor(m, 0, 1, 3, 0, 2, 3);
			des[13] = this.Minor(m, 0, 1, 2, 0, 2, 3);
			des[2] = this.Minor(m, 1, 2, 3, 0, 1, 3);
			des[6] = -this.Minor(m, 0, 2, 3, 0, 1, 3);
			des[10] = this.Minor(m, 0, 1, 3, 0, 1, 3);
			des[14] = -this.Minor(m, 0, 1, 2, 0, 1, 3);
			des[3] = -this.Minor(m, 1, 2, 3, 0, 1, 2);
			des[7] = this.Minor(m, 0, 2, 3, 0, 1, 2);
			des[11] = -this.Minor(m, 0, 1, 3, 0, 1, 2);
			des[15] = this.Minor(m, 0, 1, 2, 0, 1, 2);
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00017FFC File Offset: 0x000161FC
		private void Inverse(double[] des, double[] m)
		{
			this.Adjoint(des, m);
			double num = 1.0 / this.Determinant(m);
			for (int i = 0; i < 16; i++)
			{
				des[i] *= num;
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00018040 File Offset: 0x00016240
		private void SetRow(int i, double[] m, double[] v)
		{
			m[i] = v[0];
			m[i + 4] = v[1];
			m[i + 8] = v[2];
			m[i + 12] = v[3];
		}

		// Token: 0x04000404 RID: 1028
		private Ocean m_ocean;

		// Token: 0x04000405 RID: 1029
		private double[] m_projectorP;

		// Token: 0x04000406 RID: 1030
		private double[] m_projectorV;

		// Token: 0x04000407 RID: 1031
		private double[] m_projectorVP;

		// Token: 0x04000408 RID: 1032
		private double[] m_projectorIVP;

		// Token: 0x04000409 RID: 1033
		private double[] m_projectorR;

		// Token: 0x0400040A RID: 1034
		private double[] m_projectorI;

		// Token: 0x0400040B RID: 1035
		private List<double[]> m_frustumCorners;

		// Token: 0x0400040C RID: 1036
		private List<double[]> m_pointList;

		// Token: 0x0400040D RID: 1037
		private List<double[]> m_quad;

		// Token: 0x0400040E RID: 1038
		private List<double[]> m_corners;

		// Token: 0x0400040F RID: 1039
		private double[] MATRIX_BUFFER0;

		// Token: 0x04000410 RID: 1040
		private double[] MATRIX_BUFFER1;

		// Token: 0x04000411 RID: 1041
		private double[] VECTOR_BUFFER;

		// Token: 0x04000412 RID: 1042
		private double[] m_xaxis;

		// Token: 0x04000413 RID: 1043
		private double[] m_yaxis;

		// Token: 0x04000414 RID: 1044
		private double[] m_zaxis;

		// Token: 0x04000415 RID: 1045
		private double[] m_up;

		// Token: 0x04000416 RID: 1046
		private double[] m_a;

		// Token: 0x04000417 RID: 1047
		private double[] m_b;

		// Token: 0x04000418 RID: 1048
		private double[] m_ab;

		// Token: 0x04000419 RID: 1049
		private double[] m_pos;

		// Token: 0x0400041A RID: 1050
		private double[] m_dir;

		// Token: 0x0400041B RID: 1051
		private double[] m_lookAt;

		// Token: 0x0400041C RID: 1052
		private double[] m_p;

		// Token: 0x0400041D RID: 1053
		private double[] m_q;

		// Token: 0x0400041E RID: 1054
		private double[] m_p0;

		// Token: 0x0400041F RID: 1055
		private double[] m_p1;

		// Token: 0x04000420 RID: 1056
		private static readonly int[,] m_indices = new int[,]
		{
			{
				0,
				1
			},
			{
				1,
				2
			},
			{
				2,
				3
			},
			{
				3,
				0
			},
			{
				4,
				5
			},
			{
				5,
				6
			},
			{
				6,
				7
			},
			{
				7,
				4
			},
			{
				0,
				4
			},
			{
				1,
				5
			},
			{
				2,
				6
			},
			{
				3,
				7
			}
		};
	}
}
