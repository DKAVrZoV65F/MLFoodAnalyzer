﻿namespace Client.Extension;

public class History
{
    public int IdFood { get; set; } = 0;
    public string NameFood { get; set; } = string.Empty;
    public int IdAccount { get; set; } = 0;
    public string NickName { get; set; } = string.Empty;
    public string Old_Description { get; set; } = string.Empty;
    public string New_Description { get; set; } = string.Empty;

    public DateTime LastUpdate { get; init; } = DateTime.Now;
    public History() : this(0, "_", 0, "_", "_", "_", DateTime.Now) { }

    public History(int idFood, string nameFood, int idAccount, string nickName, string oldDescription, string newDescription, DateTime lastUpdate)
    {
        IdFood = idFood;
        NameFood = nameFood;
        IdAccount = idAccount;
        NickName = nickName;
        Old_Description = oldDescription;
        New_Description = newDescription;
        LastUpdate = lastUpdate;
    }
}
