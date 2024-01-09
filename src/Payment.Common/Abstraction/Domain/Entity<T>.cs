using Payment.Common.Abstraction.Domain.Exceptions;

namespace Payment.Common.Abstraction.Domain;

public abstract record Entity<T>(T Id) : IEntity<T>
{
  public DateTime? CreatedOn { get; set; }

  public string? CreatedBy { get; set; }

  public DateTime? ModifiedOn { get; set; }

  public string? ModifiedBy { get; set; }

  protected void CheckPolicy(IBusinessPolicy businessRule)
  {
    if (businessRule.IsInvalid())
    {
      throw new BusinessPolicyValidationException(businessRule.Message);
    }
  }
}
