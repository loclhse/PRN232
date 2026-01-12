using System;

namespace Domain.Constants
{
    public static class RoleIds
    {
        public static readonly Guid Admin = Guid.Parse("d4b8e7a0-0b6b-4e6a-9a0b-9c8d7e6f5a4b");
        public static readonly Guid Staff = Guid.Parse("c5a7d6e8-2f1b-4d3c-9b0a-8c7d6e5f4a3b");
        public static readonly Guid Customer = Guid.Parse("b6a5d4e3-1c2b-4a3d-9e0f-7b6a5c4d3e2f");
        public static readonly Guid Guest = Guid.Parse("a1b2c3d4-e5f6-4a3b-8c9d-0e1f2a3b4c5d");
    }
}
