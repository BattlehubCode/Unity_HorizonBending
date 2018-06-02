#ifndef HB_CORE_INCLUDED
#define HB_CORE_INCLUDED
#include "UnityCG.cginc"

#define POS_WORLD(vertex)
#define APPLY_OFFSET 
#define HORIZON_BEND(vertex, posWorld)
#define HB(vertex) POS_WORLD(vertex) HORIZON_BEND(vertex, posWorld)

void hb_vert(inout appdata_full v)
{
	HB(v.vertex)
}

#endif


