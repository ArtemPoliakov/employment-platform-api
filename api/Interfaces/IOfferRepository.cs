using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.OfferDtos;
using api.Models;

namespace api.Interfaces
{
    public interface IOfferRepository
    {
        Task<Offer> CreateAsync(Offer offer);
        Task<Offer> UpdateAsync(Offer offer);
        Task<List<OfferWithVacancyDto>> GetAllByJobseekerIdAsync(Guid id, bool getOnlyNonRejected);
        Task<List<OfferWithJobseekerDto>> GetAllByVacancyIdAsync(Guid id);
        Task<Offer?> GetByCompositeKeyAsync(Guid jobseekerId, Guid vacancyId);
        Task<int> DeleteAsync(Guid jobseekerId, Guid vacancyId);
    }
}