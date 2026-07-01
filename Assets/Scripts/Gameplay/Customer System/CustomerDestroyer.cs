using UnityEngine;

/// <summary>
/// お客削除ポイントに到達したお客を削除する
/// </summary>
[RequireComponent(typeof(Collider))]
public class CustomerDestroyer : MonoBehaviour
{
    public static event System.Action<MoodSetter> OnCustomerDestroy = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Customer customer))
        {
            // OutGoingステート中なら削除する
            if (customer.CurrentState.StateKey.Equals(Customer.CustomerState.OutGoing))
            {
                // 画面外の気分表示UIから気分設定を解除する
                MoodSetter moodSetter = customer.GetComponentInChildren<MoodSetter>();
                if (moodSetter != null) OnCustomerDestroy?.Invoke(moodSetter);
                // Destroy(customer.gameObject);    // 削除する代わりにプールへ戻す
            }
        }
    }
}