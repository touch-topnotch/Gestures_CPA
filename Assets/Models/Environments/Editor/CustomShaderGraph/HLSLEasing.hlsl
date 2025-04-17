//UNITY_SHADER_NO_UPGRADE
#ifndef HLSLEASING_INCLUDED
#define HLSLEASING_INCLUDED

void Elastic_float(float In, float Amplitude, out float Out)
{
    Out = sin(-13 * 3.141592/2 * (In + 1)) * pow(2, -1 * Amplitude * In) + 1; 
}

void CubicIn_float(float In, out float Out)
{
    Out = In * In * In;
}

void CubicOut_float(float In, out float Out)
{
    float f = In - 1;
    Out = f * f * f + 1;
}

void CubicInOut_float(float In, out float Out)
{
    if (In < 0.5)
    {
        Out = 4 * In * In * In;
    }
    else 
    {
        float f = 2 * In - 2;
        Out = 0.5 * f * f * f + 1;
    }
}

#endif //MYHLSLINCLUDE_INCLUDED
