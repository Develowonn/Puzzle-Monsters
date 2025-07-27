// # Unity
using UnityEngine;

// # Etc
using Cysharp.Threading.Tasks;

public class Character : MonoBehaviour
{
	public bool      IsDie  { get; protected set; }
	public bool      IsMove { get; protected set; }

	[SerializeField]
    protected float  movementTime;

	public async UniTask MoveSmoothGrid(Vector3 end)
	{
		IsMove = true;

		Vector3 start = transform.position;
		float current = 0;
		float percent = 0;

		// 객체가 삭제될 경우 비동기 함수도 종료
		var token = this.GetCancellationTokenOnDestroy();
		while (!token.IsCancellationRequested && percent < 1.0f)
		{
			current += Time.deltaTime;	
			percent  = current / movementTime;

			transform.position = Vector3.Lerp(start, end, percent);
			await UniTask.Yield();
		}
		IsMove = false;
	}
}