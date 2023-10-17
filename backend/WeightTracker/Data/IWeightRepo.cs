﻿using WeightTracker.Models.Weight;

namespace WeightTracker.Data
{
    public interface IWeightRepo
    {
        Task<bool> Add(Weight weight);
        Task<bool> WeightExistsForUserIdAndDate(int userId, DateOnly date);
    }
}