using API.Services.SentenceInformation;
using Infrastructure.Repositories.Clustering;
using Infrastructure.Repositories.Delius;
using Infrastructure.Repositories.Offloc;
using Infrastructure.Repositories.Visualisation;

namespace API.Services;

public record ApiServices(IClusteringRepository ClusteringRepository, IDeliusRepository DeliusRepository, IOfflocRepository OfflocRepository, IVisualisationRepository VisualisationRepository, AggregateService AggregateService, SentenceInformationService SentenceInformationService);
