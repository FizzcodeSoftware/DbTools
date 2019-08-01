﻿namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    public class ForeignKeyNamingMsSqlDefaultStrategy : ForeignKeyNamingDefaultStrategy
    {
        public override void SetFKName(ForeignKey fk)
        {
            if (fk.SqlTable.Name == null || fk.PrimaryKey.SqlTable.Name == null)
                return;

            var fkName = $"{fk.SqlTable.Name}{fk.PrimaryKey.SqlTable.Name}";

            var sameNameFks = fk.SqlTable.Properties.OfType<ForeignKey>().Where(fk1 =>
                fk1 != fk
                && fk1.Name?.StartsWith(fkName) == true
            ).ToList();

            var i = 1;
            foreach (var sameFk in sameNameFks)
            {
                if (sameFk.Name.Length <= fkName.Length)
                    sameFk.Name = $"{sameFk.Name}_{i++}";
            }

            fk.Name = i > 1
                ? $"{fkName}_{i}"
                : fkName;
        }
    }
}
