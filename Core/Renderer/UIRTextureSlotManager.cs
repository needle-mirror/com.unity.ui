namespace UnityEngine.UIElements.UIR
{
    class TextureSlotManager
    {
        static TextureSlotManager()
        {
            k_SlotCount = UIRenderDevice.shaderModelIs35 ? 8 : 4;
            slotIds = new int[k_SlotCount];
            for (int i = 0; i < k_SlotCount; ++i)
                slotIds[i] = Shader.PropertyToID($"_Texture{i}");
        }

        static readonly int k_SlotCount;

        internal static readonly int[] slotIds;
        internal static readonly int textureTableId = Shader.PropertyToID("_TextureInfo");

        TextureId[] m_Textures;
        uint[] m_Tickets;
        uint m_CurrentTicket;

        Vector4[] m_GpuTextures; // Contains IDs to be transferred to the GPU.

        public TextureSlotManager()
        {
            m_Textures = new TextureId[k_SlotCount];
            m_Tickets = new uint[k_SlotCount];
            m_GpuTextures = new Vector4[k_SlotCount];

            Reset();
        }

        public void Reset()
        {
            m_CurrentTicket = 0;
            for (int i = 0; i < k_SlotCount; ++i)
            {
                m_Textures[i] = TextureId.invalid;
                m_Tickets[i] = 0;
                m_GpuTextures[i] = new Vector4(-1, 1f, 1f, 0);
            }
        }

        // Returns true when the material was modified.
        public bool AssignTexture(TextureId id, MaterialPropertyBlock mat)
        {
            ++m_CurrentTicket;

            // Is the texture already bound?
            for (int i = 0; i < k_SlotCount; ++i)
            {
                if (m_Textures[i].index == id.index)
                {
                    m_Tickets[i] = m_CurrentTicket;
                    return false; // Texture is already bound.
                }
            }

            // Get the actual texture
            // We could cache this in the render chain but we shouldn't run through this often anyway.
            Texture tex = textureRegistry.GetTexture(id);

            // Find oldest slot
            uint oldestTicket = m_Tickets[0];
            int oldestSlot = 0;
            for (int i = 1; i < k_SlotCount; ++i)
            {
                if (m_Tickets[i] < oldestTicket)
                {
                    oldestTicket = m_Tickets[i];
                    oldestSlot = i;
                }
            }

            // Update the material
            m_Textures[oldestSlot] = id;
            m_GpuTextures[oldestSlot] = new Vector4(id.ConvertToGpu(), 1f / tex.width, 1f / tex.height, 0);
            mat.SetTexture(slotIds[oldestSlot], tex);
            mat.SetVectorArray(textureTableId, m_GpuTextures);

            m_Tickets[oldestSlot] = m_CurrentTicket;

            return true;
        }

        // Overridable for tests
        internal TextureRegistry textureRegistry = TextureRegistry.instance;
    }
}
