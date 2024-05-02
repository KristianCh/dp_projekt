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

	private MD5 _md5Hash;

	void Awake(){
		GameManager.AddService(this);
		DontDestroyOnLoad (gameObject);
		
		var conString = "Server=" + _Host + "," + _Port + ";" +
		                "Database=" + _Database + ";" +
		                "User ID=" + _UserId + ";" +
		                "Password=" + _Password + ";";
		
		var result = "";

		con = new SqlConnection(conString);
		con.Open();
		cmd = con.CreateCommand();

		RecordPlayerData();
		GetTopPlayers();
			
		Debug.Log(con.State);
		Debug.Log(result);
		
	}
	private void onApplicationQuit(){
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
		return con.State.ToString ();
	}

	public void RecordGameRating(string ratedWord, string answeredWord, string correctWord, string incorrectWord, float wordAoA)
	{
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var playerAge = PlayerPrefs.GetInt("Age");
		var cmdText =
			$"INSERT INTO GameRatings VALUES ('{playerGuid}', {playerAge}, '{ratedWord}', '{answeredWord}', '{correctWord}', '{incorrectWord}', {wordAoA.ToString(CultureInfo.InvariantCulture)})";
		cmd.CommandText = cmdText;
		cmd.ExecuteNonQuery();
	}

	public void RecordManualRating(string ratedWord, float wordAoA, int playerRatedAge)
	{
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var playerAge = PlayerPrefs.GetInt("Age");
		var cmdText =
			$"INSERT INTO ManualRatings VALUES ('{playerGuid}', '{ratedWord}', {playerAge}, {wordAoA.ToString(CultureInfo.InvariantCulture)}, {playerRatedAge})";
		cmd.CommandText = cmdText;
		cmd.ExecuteNonQuery();
	}

	public bool PlayerExists()
	{
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var cmdText = $"SELECT 1 FROM Players WHERE PlayerGUID='{playerGuid}';";
		cmd.CommandText = cmdText;
		rdr = cmd.ExecuteReader();
		var exists = rdr.HasRows;
		rdr.Close();
		return exists;
	}

	public void RecordPlayerData()
	{
		var playerGuid = PlayerPrefs.GetString("PlayerGUID");
		var nickname = PlayerPrefs.GetString("Nickname");
		var highscore = PlayerPrefs.GetFloat("HighScore");
		var cmdText = $"INSERT INTO Players VALUES ('{playerGuid}', '{nickname}', {highscore.ToString(CultureInfo.InvariantCulture)})";

		if (PlayerExists())
		{
			cmdText = $"UPDATE Players SET Nickname='{nickname}', Highscore={highscore.ToString(CultureInfo.InvariantCulture)} WHERE PlayerGUID='{playerGuid}'";
		}
		cmd.CommandText = cmdText;
		cmd.ExecuteNonQuery();
	}

	public List<(string nick, float highscore)> GetTopPlayers()
	{
		cmd.CommandText = "SELECT TOP 10 * FROM Players ORDER BY Highscore DESC";
		rdr = cmd.ExecuteReader();
		
		var players = new List<(string, float)>();

		while (rdr.Read())
		{
			var nick = rdr.GetString("Nickname");
			var highscore = (float)rdr.GetDouble("Highscore");
			var playerPk = rdr.GetInt32("PlayerPK");
			players.Add((string.IsNullOrEmpty(nick) ? "Hráč" + playerPk : nick, highscore));
		}
		
		rdr.Close();
		return players;
	}
}
