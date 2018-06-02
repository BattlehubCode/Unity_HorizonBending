// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#ifndef HB_CORE_INCLUDED
#define HB_CORE_INCLUDED
#include "UnityCG.cginc"

uniform float _Curvature;
uniform float _HorizonXOffset;
uniform float _HorizonYOffset;
uniform float _HorizonZOffset;
uniform float _Flatten;
uniform float4 _HBWorldSpaceCameraPos;

HB_FEATURE

float4 HorizonBendXY_ZUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d1 = max(0, abs(_HorizonYOffset - offset.y) - _Flatten);
	float d2 = max(0, abs(_HorizonXOffset - offset.x) - _Flatten);

	offset = float4(0.0f, 0.0f, (d1 * d1 + d2 * d2) * -_Curvature,  0.0f);
	return posWorld + offset;
}

float4 HorizonBendXZ_YUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d1 = max(0, abs(_HorizonZOffset - offset.z) - _Flatten);
	float d2 = max(0, abs(_HorizonXOffset - offset.x) - _Flatten);

	offset = float4(0.0f, (d1 * d1 + d2 * d2) * -_Curvature, 0.0f, 0.0f);
	return posWorld + offset;
}

float4 HorizonBendYZ_XUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d1 = max(0, abs(_HorizonZOffset - offset.z) - _Flatten);
	float d2 = max(0, abs(_HorizonYOffset - offset.y) - _Flatten);

	offset = float4((d1 * d1 + d2 * d2) * -_Curvature, 0.0f, 0.0f, 0.0f);
	return posWorld + offset;
}

float4 HorizonBendX_YUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d = max(0, abs(_HorizonZOffset - offset.z) - _Flatten);
	offset = float4(0.0f, d * d * -_Curvature, 0.0f, 0.0f);

	return posWorld + offset;
}

float4 HorizonBendX_ZUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d = max(0, abs(_HorizonYOffset - offset.y) - _Flatten);
	offset = float4(0.0f, 0.0f, d * d * -_Curvature,  0.0f);

	return posWorld + offset;
}

float4 HorizonBendY_XUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d = max(0, abs(_HorizonZOffset - offset.z) - _Flatten);
	offset = float4(d * d * -_Curvature, 0.0f, 0.0f, 0.0f);

	return posWorld + offset;
}

float4 HorizonBendY_ZUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d = max(0, abs(_HorizonXOffset - offset.x) - _Flatten);
	offset = float4(0.0f, 0.0f, d * d * -_Curvature, 0.0f);

	return posWorld + offset;
}

float4 HorizonBendZ_YUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d = max(0, abs(_HorizonXOffset - offset.x) - _Flatten);
	offset = float4(0.0f, d * d * -_Curvature, 0.0f, 0.0f);

	return posWorld + offset;
}

float4 HorizonBendZ_XUP(float4 posWorld)
{
	float4 offset = posWorld - _HBWorldSpaceCameraPos;

	float d = max(0, abs(_HorizonYOffset - offset.y) - _Flatten);
	offset = float4(d * d * -_Curvature, 0.0f, 0.0f, 0.0f);

	return posWorld + offset;
}

#if _HB_XZ_YUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(0.0f, _HorizonYOffset, 0.0f, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendXZ_YUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_X_YUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(0.0f, _HorizonYOffset, 0.0f, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendX_YUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_Z_YUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(0.0f, _HorizonYOffset, 0.0f, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendZ_YUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_XY_ZUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(0.0f,  0.0f, _HorizonZOffset, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendXY_ZUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_X_ZUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(0.0f,  0.0f, _HorizonZOffset, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendX_ZUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_Y_ZUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(0.0f,  0.0f, _HorizonZOffset, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendY_ZUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_YZ_XUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(_HorizonXOffset, 0.0f,  0.0f, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendYZ_XUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_Y_XUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(_HorizonXOffset, 0.0f,  0.0f, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendY_XUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#elif _HB_Z_XUP
	#define POS_WORLD(vertex) float4 posWorld = mul(unity_ObjectToWorld, vertex);
	#define APPLY_OFFSET posWorld += float4(_HorizonXOffset, 0.0f,  0.0f, 0.0f);
	#define HORIZON_BEND(vertex, posWorld) posWorld = HorizonBendZ_XUP(posWorld); vertex = mul(unity_WorldToObject, posWorld);

#else
	#define POS_WORLD(vertex)
	#define APPLY_OFFSET 
	#define HORIZON_BEND(vertex, posWorld)

#endif

#define HB(vertex) POS_WORLD(vertex) HORIZON_BEND(vertex, posWorld)

void hb_vert(inout appdata_full v)
{
	HB(v.vertex)
}

#endif


