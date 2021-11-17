using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RMuler.Db
{
    public class CharacterStash
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string ImageSrc { get; set; }
        public DateTime UpdateDate { get; set; }

        public CharacterStash() { Name = "test"; ImageSrc = ""; UpdateDate = DateTime.Now; }
        public CharacterStash(string name)
        {
            Name = name;
            UpdateDate = DateTime.Now;
            ImageSrc = "";
        }

        public void Save()
        {
            if (Id == null)
            {
                Insert();
            }
            else
            {
                Update();
            }

        }

        private void Insert()
        {
            using (var connection = DB.Connection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    insert into character_stash(name, image_src, update_date) 
                    values (@name, @imageSrc, @updateDate);
                ";
                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@imageSrc", ImageSrc);
                command.Parameters.AddWithValue("@updateDate", UpdateDate);
                
                command.ExecuteNonQuery();
            }
        }

        private void Update()
        {
            using (var connection = DB.Connection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                UpdateDate = DateTime.Now;
                command.CommandText =
                @"
                    update character_stash
                        set name = @name, 
                        image_src = @imageSrc, 
                        update_date = @updateDate
                    where id = @id;
                ";
                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@imageSrc", ImageSrc);
                command.Parameters.AddWithValue("@updateDate", UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@id", Id);


                command.ExecuteNonQuery();
            }
        }

        public void Delete()
        {
            using (var connection = DB.Connection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    delete from character_stash
                           where id = @id;
                ";
                command.Parameters.AddWithValue("@id", Id);


                command.ExecuteNonQuery();
            }
        }

        public static List<CharacterStash> FindAll()
        {
            var list = new List<CharacterStash>();
            using (var connection = DB.Connection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    select * from character_stash order by update_date desc;
                ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var character = new CharacterStash(Convert.ToString(reader["name"]));
                        character.Id = Convert.ToInt32(reader["id"]);
                        character.UpdateDate = Convert.ToDateTime(reader["update_date"]);
                        character.ImageSrc = Convert.ToString(reader["image_src"]);
                        list.Add(character);
                    }
                }
            }
            return list;
        }
    }
}
