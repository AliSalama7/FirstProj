﻿namespace Movies.Domain.Models
{
    public  class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ProfilepictureUrl { get; set; }
        public string CharacterName { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; }

    }
}
