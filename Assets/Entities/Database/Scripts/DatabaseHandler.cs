﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Entities.Database;
using Entities.GameManagement;

public class DatabaseHandler : MonoBehaviour, IService
{
	[SerializeField]
	private string _Host;
	
	[SerializeField]
	private int _Port;

	[SerializeField]
	private string _Database;

	[SerializeField]
	private string _UserId;
	
	[SerializeField]
	private string _Password;

	private string connectionString;
	private SqlConnection con = null;
	private SqlCommand cmd = null;
	private SqlDataReader rdr = null;

	private bool _playerExistsCache = false;
	private PlayerDataManager _playerDataManager;

	private MD5 _md5Hash;
	private string ConString => "Server=" + _Host + "," + _Port + ";" +
	                            "Database=" + _Database + ";" +
	                            "User ID=" + _UserId + ";" +
	                            "Password=" + _Password + ";";

	private SqlConnection Con {
		get
		{
			if (GetConnectionState() == ConnectionState.Closed)
			{
				con = new SqlConnection(ConString);
				con.Open();
			}

			return con;
		}
	}

	private SqlCommand Cmd => cmd ??= Con.CreateCommand();

	private void Awake()
	{
		GameManager.AddService(this);
	}

	private void Start()
	{
		_playerDataManager = GameManager.GetService<PlayerDataManager>();
		Task.Run(Initialize);
	}

	private async UniTask Initialize()
	{
		con = new SqlConnection(ConString);
		Debug.Log("Opening Connection");
		var retryConnection = true;
		while (retryConnection)
		{
			retryConnection = false;
			try
			{
				await con.OpenAsync();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				Debug.Log("Database unavailable, retrying...");
				retryConnection = true;
			}
			await Task.Delay(5000);
		}
		Debug.Log("Connection Opened");
		
		Debug.Log("Recording PlayerData");
		await RecordPlayerDataInternal();
		Debug.Log("Recorded PlayerData");
	}
	
	private void OnApplicationQuit(){
		if (con != null) {
			if (con.State.ToString () != "Closed") {
				con.Close ();
				Debug.Log ("Mysql connection closed");
			}
			con.Dispose ();
		}
	}
	
	public ConnectionState GetConnectionState(){
		if (con == null) return ConnectionState.Closed;
		return con.State;
	}

	public async void RecordGameRating(string ratedWord, string answeredWord, string correctWord, string incorrectWord,
		float wordAoA)
	{
		await Task.Run(() => RecordGameRatingInternal(ratedWord, answeredWord, correctWord, incorrectWord, wordAoA));
	}

	public async UniTask RecordGameRatingInternal(string ratedWord, string answeredWord, string correctWord, string incorrectWord, float wordAoA)
	{
		var playerGuid = _playerDataManager.PlayerGuid;
		var playerAge = _playerDataManager.PlayerAge;
		var cmdText =
			$"INSERT INTO GameRatings VALUES ('{playerGuid}', {playerAge}, '{ratedWord}', '{answeredWord}', '{correctWord}', '{incorrectWord}', {wordAoA.ToString(CultureInfo.InvariantCulture)})";
		Cmd.CommandText = cmdText;
		await Cmd.ExecuteNonQueryAsync();
	}

	public async void RecordManualRating(string ratedWord, float wordAoA, int playerRatedAge)
	{
		await Task.Run(() => RecordManualRatingInternal(ratedWord, wordAoA, playerRatedAge));
	}
	
	private async UniTask RecordManualRatingInternal(string ratedWord, float wordAoA, int playerRatedAge)
	{
		var playerGuid = _playerDataManager.PlayerGuid;
		var playerAge = _playerDataManager.PlayerAge;
		var cmdText =
			$"INSERT INTO ManualRatings VALUES ('{playerGuid}', '{ratedWord}', {playerAge}, {wordAoA.ToString(CultureInfo.InvariantCulture)}, {playerRatedAge})";
		Cmd.CommandText = cmdText;
		await Cmd.ExecuteNonQueryAsync();
	}

	public async void RecordPlayerData()
	{ 
		await RecordPlayerDataInternal();
	}

	private async UniTask RecordPlayerDataInternal()
	{
		var playerGuid = _playerDataManager.PlayerGuid;
		var nickname = _playerDataManager.PlayerNickname;
		var highscore = _playerDataManager.PlayerHighscore;
		var cmdText = $"INSERT INTO Players VALUES ('{playerGuid}', '{nickname}', {highscore.ToString(CultureInfo.InvariantCulture)})";

		var playerExists = await CheckPlayerExists();
		if (playerExists)
			cmdText = $"UPDATE Players SET Nickname='{nickname}', Highscore={highscore.ToString(CultureInfo.InvariantCulture)} WHERE PlayerGUID='{playerGuid}'";
		
		Cmd.CommandText = cmdText;
		await Cmd.ExecuteNonQueryAsync();
	}

	public async UniTask<List<(string guid, string nick, float highscore)>> GetTopPlayers()
	{
		Cmd.CommandText = "SELECT TOP 10 * FROM Players ORDER BY Highscore DESC";
		rdr = await Cmd.ExecuteReaderAsync();
		
		var players = new List<(string, string, float)>();

		while (await rdr.ReadAsync())
		{
			var guid = rdr.GetString("PlayerGUID");
			var nick = rdr.GetString("Nickname");
			var highscore = (float)rdr.GetDouble("Highscore");
			var playerPk = rdr.GetInt32("PlayerPK");
			players.Add((guid, string.IsNullOrEmpty(nick) ? "Hráč" + playerPk : nick, highscore));
		}
		
		CloseReader(rdr);
		return players;
	}

	private async UniTask<bool> CheckPlayerExists()
	{
		if (_playerExistsCache) return true;
		var playerGuid = _playerDataManager.PlayerGuid;
		var cmdText = $"SELECT * FROM Players WHERE PlayerGUID='{playerGuid}';";
		Cmd.CommandText = cmdText;
		rdr = await Cmd.ExecuteReaderAsync();
		_playerExistsCache = rdr.HasRows;

		var playerPk = 0;
		while (await rdr.ReadAsync())
		{
			playerPk = rdr.GetInt32("PlayerPK");
		}
		_playerDataManager.PlayerPk = playerPk;
		CloseReader(rdr);
		return _playerExistsCache;
	}

	private async void CloseReader(SqlDataReader rdr)
	{
		await rdr.CloseAsync();
	}
}
