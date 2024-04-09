using System;

namespace Auth.MicroService.Domain.Entities
{
    public class RefreshToken
    {
        public int? RefreshTokenId { get; private set; }
        public int UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresIn { get; private set; }
        public DateTime CreatedDateUtc { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime? RevokedDateUtc { get; private set; }
        public virtual User User { get; private set; }

        private RefreshToken(
            int? refreshTokenId,
            int userId,
            string token,
            DateTime expiresIn,
            DateTime createdDateUtc,
            bool isRevoked,
            DateTime? revokedDateUtc)
        {
            this.RefreshTokenId = refreshTokenId;
            this.UserId = userId;
            this.Token = token;
            this.ExpiresIn = expiresIn;
            this.CreatedDateUtc = createdDateUtc;
            this.IsRevoked = isRevoked;
            this.RevokedDateUtc = revokedDateUtc;
        }

        private static RefreshToken Create(
            int? refreshTokenId,
            int userId,
            string token,
            DateTime expiresIn,
            DateTime createdDateUtc,
            bool isRevoked,
            DateTime? revokedDateUtc)
        {
            return new RefreshToken(
                refreshTokenId,
                userId,
                token,
                expiresIn,
                createdDateUtc,
                isRevoked,
                revokedDateUtc);
        }

        public static RefreshToken CreateNewRefreshToken(
            int? refreshTokenId,
            int userId,
            string token,
            DateTime expiresIn)
        {
            return Create(
                refreshTokenId,
                userId,
                token,
                expiresIn,
                DateTime.UtcNow,
                false,
                null);
        }
        
        public RefreshToken UpdateRevokeStatus(
            bool isRevoked,
            DateTime revokedDateUtc)
        {
           this.IsRevoked = isRevoked;
           this.RevokedDateUtc = revokedDateUtc;

            return this;
        }
        
        public RefreshToken UpdateExpiresIn(
            DateTime expiresIn)
        {
            this.ExpiresIn = expiresIn;

            return this;
        }

        public static RefreshToken CreateRefreshTokenForTests(
            int userId,
            string token,
            DateTime expiresIn,
            DateTime createdDateUtc,
            bool isRevoked,
            DateTime? revokedDateUtc)
        {
            return RefreshToken.Create(
                9999,
                userId,
                token,
                expiresIn,
                createdDateUtc,
                isRevoked,
                revokedDateUtc);
        }
    }
}