using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using ETModel;

public class TestAsync : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Start Func Begin");
        // StartCoroutine(TestAsyncFunc());
        TestAsyncFunc2().Coroutine();
        Debug.Log("Start Func End");
    }

    private IEnumerator TestAsyncFunc()
    {
        Debug.Log("Enter TestAsyncFunc");
        yield return new WaitForSeconds(2);
        Debug.Log("After 2 Seconds");
    }

    // private async void TestAsyncFunc2()
    private async ETVoid TestAsyncFunc2()
    {
        Debug.Log("Enter TestAsyncFunc2");
        await this.TestAsyncFunc3();
        Debug.Log("After 2 Seconds");
        int result = await this.TestAsyncReturnResult();
        Debug.Log($"4秒后结果是{result}");
    }

    // private async Task TestAsyncFunc3()
    private async ETTask TestAsyncFunc3()
    {
        await Task.Delay(2000);
    }

    // private async Task<int> TestAsyncReturnResult()
    private async ETTask<int> TestAsyncReturnResult()
    {
        await Task.Delay(2000);
        return 100;
    }
}
