using System.Threading.Tasks;
using PaintShopIMS.Models;
using PaintShopIMS.Repositories;

namespace PaintShopIMS.Services
{
    public class PartService
    {
        private readonly PartRepository _partRepo;

        public PartService(PartRepository partRepo)
        {
            _partRepo = partRepo;
        }

        public async Task<(bool ok, string msg)> AddPart(PartMaster part)
        {
            if (string.IsNullOrWhiteSpace(part.PartNo))
                return (false, "Part No is required.");
            
            if (string.IsNullOrWhiteSpace(part.PartName))
                return (false, "Part Name is required.");

            bool exists = await _partRepo.ExistsPartNoAsync(part.PartNo);
            if (exists)
                return (false, "Part No already exists in the system.");

            await _partRepo.InsertAsync(part);
            return (true, "Part added successfully.");
        }

        public async Task<(bool ok, string msg)> UpdatePart(PartMaster part)
        {
            if (string.IsNullOrWhiteSpace(part.PartNo))
                return (false, "Part No is required.");
            
            if (string.IsNullOrWhiteSpace(part.PartName))
                return (false, "Part Name is required.");

            bool exists = await _partRepo.ExistsPartNoExcludingAsync(part.PartNo, part.PartId);
            if (exists)
                return (false, "Another part with this Part No already exists.");

            await _partRepo.UpdateAsync(part);
            return (true, "Part updated successfully.");
        }

        public async Task<(bool ok, string msg)> DeletePart(int partId)
        {
            await _partRepo.DeleteAsync(partId);
            return (true, "Part deleted successfully.");
        }
    }
}
