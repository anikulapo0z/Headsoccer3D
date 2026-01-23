#ifndef SHADER_UTILITY_VORONOI_INCLUDED
#define SHADER_UTILITY_VORONOI_INCLUDED

#include "ShaderUtility.hlsl"

struct VoronoiParams
{
    float randomness;
    float smoothness;
    float scale;
};
struct VoronoiData2D
{
    float3 color;
    float2 cellPosition;
    float distance;
};

struct VoronoiData3D
{
    float3 color;
    float3 cellPosition;
    float2 cellPositionUV;
    float distance;
};

//sourced fron the internet
//I have spent enough time looking at Voronoi math, I'd rather grab something from internet than learn math by myself
//I love my mental health, I will save maths for later when I get a mentor
//or when I have leisure time, heaven knows when I will have that
//https://www.ronja-tutorials.com/post/028-voronoi-noise/
float2 voronoiNoise2D(float2 value)
{
    float2 baseCell = floor(value);
    float minDistToCell = 10;
				//location for the nearest neighbour for this pixel's blob'
    float2 closestCell;
				//We tell the compiler to unroll the loops to get better performance in the shader.
				[unroll]
    for (int x = -1; x <= 1; x++)
    {
					[unroll]
        for (int y = -1; y <= 1; y++)
        {
						//checking neighbouring cell
            float2 cell = baseCell + float2(x, y);
						//get a random position inside each cell we made from flooring
            float2 cellPosition = cell + rand2dTo2d(cell);
						//for this pixel, find the Vector towards that random center we just found
            float2 toCell = cellPosition - value;
						//distance
            float distToCell = length(toCell);
						//smaller than the previous closest cell position, if it is, we replace the distance.
            if (distToCell < minDistToCell)
            {
                minDistToCell = distToCell;
                closestCell = cell;
            }
        }
    }
    float random = rand2dTo1d(closestCell);
    return float2(minDistToCell, random);
}
//this one is just shamelessly copied
VoronoiData3D voronoiNoise3D(float3 value)
{
    float3 baseCell = floor(value);

    //first pass to find the closest cell
    float minDistToCell = 10;
    float3 toClosestCell;
    float3 closestCell;
    
    VoronoiData3D voronoiData;
    
    [unroll]
    for (int x1 = -1; x1 <= 1; x1++)
    {
        [unroll]
        for (int y1 = -1; y1 <= 1; y1++)
        {
            [unroll]
            for (int z1 = -1; z1 <= 1; z1++)
            {
                float3 cell = baseCell + float3(x1, y1, z1);
                float3 cellPosition = cell + rand3dTo3d(cell);
                float3 toCell = cellPosition - value;
                float distToCell = length(toCell);
                if (distToCell < minDistToCell)
                {
                    minDistToCell = distToCell;
                    closestCell = cell;
                    voronoiData.cellPosition = closestCell;
                    toClosestCell = toCell;
                }
            }
        }
    }

    //second pass to find the distance to the closest edge
    float minEdgeDistance = 10;
    [unroll]
    for (int x2 = -1; x2 <= 1; x2++)
    {
        [unroll]
        for (int y2 = -1; y2 <= 1; y2++)
        {
            [unroll]
            for (int z2 = -1; z2 <= 1; z2++)
            {
                float3 cell = baseCell + float3(x2, y2, z2);
                float3 cellPosition = cell + rand3dTo3d(cell);
                float3 toCell = cellPosition - value;

                float3 diffToClosestCell = abs(closestCell - cell);
                bool isClosestCell = diffToClosestCell.x + diffToClosestCell.y + diffToClosestCell.z < 0.1;
                if (!isClosestCell)
                {
                    float3 toCenter = (toClosestCell + toCell) * 0.5;
                    float3 cellDifference = normalize(toCell - toClosestCell);
                    float edgeDistance = dot(toCenter, cellDifference);
                    minEdgeDistance = min(minEdgeDistance, edgeDistance);
                }
            }
        }
    }

    float random = rand3dTo1d(closestCell);
    voronoiData.distance = minEdgeDistance;
    return voronoiData;
}

VoronoiData2D voronoiModified2D(float2 value, float2 seed, float jaggedMult, float2 uv)
{
    float2 baseCell = floor(value);
    float minDistToCell = 10;
	//location for the nearest neighbour for this pixel's blob'
    float2 closestCell;
    float2 toClosestCell;
    VoronoiData2D voronoiData;
    voronoiData.cellPosition = uv;
    
	//We tell the compiler to unroll the loops to get better performance in the shader.
	[unroll]
    for (int x = -1; x <= 1; x++)
    {
		[unroll]
        for (int y = -1; y <= 1; y++)
        {
			//checking neighbouring cell
            float2 cell = baseCell + float2(x, y);
			//get a random position inside each cell we made from flooring
            float2 cellPosition = cell + rand2dTo2d(cell);
			//for this pixel, find the Vector towards that random center we just found
            float2 toCell = cellPosition - value;
			//distance
            float distToCell = length(toCell);
            //disturb the dist with noise to painterly blob
            distToCell = distToCell + (rand2dTo1d(floor(seed) + float2(x, y)) * jaggedMult);
			//smaller than the previous closest cell position, if it is, we replace the distance.
            if (distToCell < minDistToCell)
            {
                minDistToCell = distToCell;
                closestCell = cell;
                voronoiData.cellPosition = closestCell;
                toClosestCell = toCell;
            }
        }
    }
    voronoiData.color = rand2dTo1d(closestCell);
    voronoiData.distance = minDistToCell;
    return voronoiData;
}


VoronoiData2D voronoiModified2D(float2 value, float2 seed, float jaggedMult, float2 uv, float detail)
{
    float2 baseCell = floor(value) + floor(detail);
    float minDistToCell = 10;
	//location for the nearest neighbour for this pixel's blob'
    float2 closestCell;
    float2 toClosestCell;
    VoronoiData2D voronoiData;
    voronoiData.cellPosition = uv;
    
	//We tell the compiler to unroll the loops to get better performance in the shader.
	[unroll]
    for (int x = -1; x <= 1; x++)
    {
		[unroll]
        for (int y = -1; y <= 1; y++)
        {
			//checking neighbouring cell
            float2 cell = baseCell + float2(x, y);
			//get a random position inside each cell we made from flooring
            float2 cellPosition = cell + rand2dTo2d(cell);
			//for this pixel, find the Vector towards that random center we just found
            float2 toCell = cellPosition - value;
			//distance
            float distToCell = length(toCell);
            //disturb the dist with noise to painterly blob
            distToCell = distToCell + (rand2dTo1d(floor(seed) + float2(x, y)) * jaggedMult);
			//smaller than the previous closest cell position, if it is, we replace the distance.
            if (distToCell < minDistToCell)
            {
                minDistToCell = distToCell;
                closestCell = cell;
                voronoiData.cellPosition = closestCell;
                toClosestCell = toCell;
            }
        }
    }
    voronoiData.color = rand2dTo1d(closestCell);
    voronoiData.distance = minDistToCell;
    return voronoiData;
}


VoronoiData3D voronoiModified3D(float3 value, float3 seed, float jaggedMult, float2 uv)
{
    float3 baseCell = floor(value);

    //first pass to find the closest cell
    float minDistToCell = 10;
    float3 toClosestCell;
    float3 closestCell;
    
    VoronoiData3D voronoiData;
    
    [unroll]
    for (int x1 = -1; x1 <= 1; x1++)
    {
        [unroll]
        for (int y1 = -1; y1 <= 1; y1++)
        {
            [unroll]
            for (int z1 = -1; z1 <= 1; z1++)
            {
                float3 cell = baseCell + float3(x1, y1, z1);
                float3 cellPosition = cell + rand3dTo3d(cell);
                float3 toCell = cellPosition - value;
                float distToCell = length(toCell);
                distToCell = distToCell + (rand3dTo1d(floor(seed) + float3(x1, y1, z1) ) * jaggedMult);
                if (distToCell < minDistToCell)
                {
                    minDistToCell = distToCell;
                    closestCell = cell;
                    voronoiData.cellPosition = closestCell;
                    voronoiData.cellPositionUV = uv;
                    toClosestCell = toCell;
                }
            }
        }
    }

    voronoiData.color = rand3dTo3d(closestCell);
    voronoiData.distance = minDistToCell;
    return voronoiData;
}


//hash_int3_to_float3, voronoi_distance, voronoi_smooth_f1 by Detox from Painterly Normals Shader in Unity Asset Store
float3 hash_int3_to_float3(int3 p)
{
    uint3 q = uint3(p) * uint3(1597334673u, 3812015801u, 2912667907u);
    q = (q.x ^ q.y ^ q.z) * uint3(1597334673u, 3812015801u, 2912667907u);
    return float3(q) / float(0xFFFFFFFFu);
}

float voronoi_distance(float3 a, float3 b, VoronoiParams params)
{
    float3 d = abs(a - b);
    return length(d);
}
//further modified by me - Prasin
VoronoiData3D voronoi_smooth_f1(VoronoiParams params, float3 coord, float3 noise, float jaggedMult)
{
    coord *= params.scale;
    
    const float3 cellPosition_f = floor(coord);
    const float3 localPosition = coord - cellPosition_f;
    const int3 cellPosition = int3(cellPosition_f);

    float smoothDistance = 0.0;
    float3 smoothColor = float3(0.0, 0.0, 0.0);
    float3 smoothPosition = float3(0.0, 0.0, 0.0);
    float h = -1.0;
    
    [unroll]
    for (int k = -2; k <= 2; k++)
    {
        [unroll]
        for (int j = -2; j <= 2; j++)
        {
            [unroll]
            for (int i = -2; i <= 2; i++)
            {
                const int3 cellOffset = int3(i, j, k);
                const float3 pointPosition = float3(cellOffset) +
                                           hash_int3_to_float3(cellPosition + cellOffset) *
                                           params.randomness;
                const float distanceToPoint = voronoi_distance(pointPosition, localPosition, params) + (rand3dTo1d(floor(noise) + float3(k, j, i)) * jaggedMult);
                h = h == -1.0 ?
                    1.0 :
                    smoothstep(0.0, 1.0, 0.5 + 0.5 * (smoothDistance - distanceToPoint) / params.smoothness);
                
                float correctionFactor = params.smoothness * h * (1.0 - h);
                smoothDistance = lerp(smoothDistance, distanceToPoint, h) - correctionFactor;
                correctionFactor /= 1.0 + 3.0 * params.smoothness;
                
                const float3 cellColor = hash_int3_to_float3(cellPosition + cellOffset);
                smoothColor = lerp(smoothColor, cellColor, h) - correctionFactor;
                smoothPosition = lerp(smoothPosition, pointPosition, h) - correctionFactor;
            }
        }
    }

    VoronoiData3D output;
    output.distance = smoothDistance / params.scale;
    output.color = smoothColor;
    output.cellPosition = (cellPosition_f + smoothPosition) / params.scale;
    return output;
}

#endif // SHADER_UTILITY_VORONOI_INCLUDED
