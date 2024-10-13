namespace Reklamna_agencija.DTOs
{
    public class KorisnikDto
    {
        public int Id { get; set; }
        public string? AccessToken { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }

        public KorisnikDto(string AccessToken, string email, string Role, int Id)
        {
            this.Id = Id;
            this.AccessToken = AccessToken;
            this.Email = email;
            this.Role = Role;
        }
    }
}
