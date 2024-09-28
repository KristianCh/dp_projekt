using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
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

	private MD5 _md5Hash;
	private string ConString => "Server=" + _Host + "," + _Port + ";" +
	                            "Database=" + _Database + ";" +
	                            "User ID=" + _UserId + ";" +
	                            "Password=" + _Password + ";";

	private SqlConnection Con {
		get
		{
			if (GetConnectionState() == "Closed")
			{
				con = new SqlConnection(ConString);
				con.Open();
			}

			return con;
		}
	}

	private SqlCommand Cmd => cmd ??= Con.CreateCommand();
	private bool PlayerExists => _playerExistsCache || CheckPlayerExists();

	void Awake(){
		GameManager.AddService(this);
		DontDestroyOnLoad (gameObject);

		RecordPlayerData();
		CheckPlayerExists();
			
		Debug.Log(Con.State);
		
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
	
	public string GetConnectionState(){
		if (con == null) return "Closed";
		return con.State.ToString();
	}

	public void RecordGameRating(string ratedWord, string answeredWord, string correctWord, string incorrectWord, float wordAoA)
	{
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var playerAge = PlayerPrefs.GetInt("Age");
		var cmdText =
			$"INSERT INTO GameRatings VALUES ('{playerGuid}', {playerAge}, '{ratedWord}', '{answeredWord}', '{correctWord}', '{incorrectWord}', {wordAoA.ToString(CultureInfo.InvariantCulture)})";
		Cmd.CommandText = cmdText;
		Cmd.ExecuteNonQueryAsync();
	}

	public void RecordManualRating(string ratedWord, float wordAoA, int playerRatedAge)
	{
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var playerAge = PlayerPrefs.GetInt("Age");
		var cmdText =
			$"INSERT INTO ManualRatings VALUES ('{playerGuid}', '{ratedWord}', {playerAge}, {wordAoA.ToString(CultureInfo.InvariantCulture)}, {playerRatedAge})";
		Cmd.CommandText = cmdText;
		Cmd.ExecuteNonQueryAsync();
	}

	public bool CheckPlayerExists()
	{
		if (_playerExistsCache) return true;
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var cmdText = $"SELECT * FROM Players WHERE PlayerGUID='{playerGuid}';";
		Cmd.CommandText = cmdText;
		rdr = Cmd.ExecuteReader();
		_playerExistsCache = rdr.HasRows;

		var playerPk = 0;
		while (rdr.Read())
		{
			playerPk = rdr.GetInt32("PlayerPK");
		}
		PlayerPrefs.SetInt("PlayerPK", playerPk);
		CloseReader(rdr);
		return _playerExistsCache;
	}

	public void RecordPlayerData()
	{
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var nickname = PlayerPrefs.GetString("Nickname");
		var highscore = PlayerPrefs.GetFloat("HighScore");
		var cmdText = $"INSERT INTO Players VALUES ('{playerGuid}', '{nickname}', {highscore.ToString(CultureInfo.InvariantCulture)})";

		StartCoroutine(RecordPlayerDataAsync());
		IEnumerator RecordPlayerDataAsync()
		{
			if (PlayerExists)
				cmdText = $"UPDATE Players SET Nickname='{nickname}', Highscore={highscore.ToString(CultureInfo.InvariantCulture)} WHERE PlayerGUID='{playerGuid}'";
			
			Cmd.CommandText = cmdText;
			Cmd.ExecuteNonQueryAsync();	
			yield break;
		}
	}

	public List<(string guid, string nick, float highscore)> GetTopPlayers()
	{
		Cmd.CommandText = "SELECT TOP 10 * FROM Players ORDER BY Highscore DESC";
		rdr = Cmd.ExecuteReader();
		
		var players = new List<(string, string, float)>();

		while (rdr.Read())
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

	private void CloseReader(SqlDataReader rdr)
	{
		if (Con.State.ToString () == "Open")
			rdr.Close();
	}
}
