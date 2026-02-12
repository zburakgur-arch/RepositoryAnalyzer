using System;
using RepositoryAnalyzer.Domain.Entities;

namespace RepositoryAnalyzer.Domain.Aggregates;

public interface IAggregate<T> : IEntity<T>
{

}
