public abstract class BindingPlugin
{
	public abstract void Initialize();

	public abstract void Review(string defaultComment);

	public abstract void CreateInAppPurchase(string delegator);

	public abstract void ClearProductsIdentifier();

	public abstract void AddProductsIdentifier(string productsId);

	public abstract void RequestProductsInfo();

	public abstract string GetProductInfoPrice(string productsId);

	public abstract string GetProductInfoTitle(string productsId);

	public abstract void BuyProduct(string productsId);

	public abstract bool CanMakePayments();

	public abstract string GetPurchasedTransaction();

	public abstract string GetProductIdentifier(string transactionId);

	public abstract string GetTransactionReceipt(string transactionId);

	public abstract void FinishTransaction(string transactionId);

	public abstract void ResetPaymentQueueDelegate();

	public abstract string GetNoticeRegistrationId();

	public abstract string GetUrlSchemeStr();

	public abstract void ClearUrlSchemeStr();

	public abstract void RegistPnote(string guid);

	public abstract void UnregistPnote();

	public abstract void SendMessagePnote(string message, string sender, string reciever, string launchOption);

	public abstract void RegistTagsPnote(string tags, string guid);

	public abstract string GetPnoteLaunchString();

	public abstract void ClearIconBadgeNumber();

	public abstract void GetSystemProxy(out string host, out ushort port);

	public abstract void SetClipBoard(string text);
}
