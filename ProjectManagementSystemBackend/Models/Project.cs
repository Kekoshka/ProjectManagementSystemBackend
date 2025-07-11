﻿namespace ProjectManagementSystemBackend.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Security {  get; set; }
        public ICollection<BaseBoard>? BaseBoards { get; set; }
        public ICollection<Participant>? Participants { get; set; }
    }
}
