namespace picpay_challenge.Domain.Services.Interfaces
{
    public interface IServiceInterface<G>
    {
        List<G>? FindMany();
        G? FindById(int Id);
        //G Create(G payload);

    }
}
