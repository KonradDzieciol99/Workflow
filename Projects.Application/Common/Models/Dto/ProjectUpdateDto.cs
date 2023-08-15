using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Application.Common.Models.Dto;

internal record ProjectUpdateDto(string? Name, string? IconUrl, string? NewLeaderId);
