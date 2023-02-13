using System.Collections.Generic;
using UnityEngine;

public abstract class SocialPlatform : MonoBehaviour
{
	public abstract void Initialize(GameObject callbackObject);

	public abstract void Login(GameObject callbackObject);

	public abstract void Logout();

	public abstract void RequestMyProfile(GameObject callbackObject);

	public abstract void RequestFriendList(GameObject callbackObject);

	public abstract void SetScore(SocialDefine.ScoreType type, int score, GameObject callbackObject);

	public abstract void CreateMyGameData(string gameId, GameObject callbackObject);

	public abstract void RequestGameData(string userId, GameObject callbackObject);

	public abstract void DeleteGameData(GameObject callbackObject);

	public abstract void InviteFriend(GameObject callbackObject);

	public abstract void SendEnergy(SocialUserData userData, GameObject callbackObject);

	public abstract void ReceiveEnergy(string energyId, GameObject callbackObject);

	public abstract void Feed(string feedCaption, string feedText, GameObject callbackObject);

	public abstract void RequestInvitedFriend(GameObject callbackObject);

	public abstract void RequestPermission(GameObject callbackObject);

	public abstract void AddPermission(List<SocialInterface.Permission> permissions, GameObject callbackObject);
}
