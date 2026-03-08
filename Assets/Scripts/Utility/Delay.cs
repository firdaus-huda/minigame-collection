using System;
using Cysharp.Threading.Tasks;

public static class Delay
{
    public static async UniTask Seconds(float seconds)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(seconds));
    }
    
    public static async UniTask Minutes(float minutes)
    {
        await UniTask.Delay(TimeSpan.FromMinutes(minutes));
    }
}