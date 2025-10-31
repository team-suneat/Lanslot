namespace TeamSuneat
{
	public enum ItemCategories
	{
		None,

		Sword,      // ��
		Knife,      // �ܵ�
		Axe,        // ����
		Blunt,      // �б�
		Javelin,    // â
		Talisman,   // Ż������
		Focus,      // ���

		//������������������������������������������������������������������������������������������������������������������������������������������������-

		Helmet, // ����
		Armor,  // ����
		Belt,   // �㸮��
		Gloves, // �尩
		Boots,  // ��ȭ

		//������������������������������������������������������������������������������������������������������������������������������������������������-

		Accessories,    // ��ű�

		//������������������������������������������������������������������������������������������������������������������������������������������������-

		Quest,              // ����Ʈ
		CraftingMaterial,   // ���� ���
		Refill,             // ������
		Essence,            // ����
		SkillBook,          // ����� ��
		Prayers,            // �⵵ (���� ��ȭ+)
		RiftGem,
	}

	public static class ItemCategoryChecker
	{
		public static bool IsEquippable(this ItemCategories key)
		{
			switch (key)
			{
				case ItemCategories.Sword:
				case ItemCategories.Knife:
				case ItemCategories.Axe:
				case ItemCategories.Blunt:
				case ItemCategories.Javelin:
				case ItemCategories.Talisman:
				case ItemCategories.Focus:

				case ItemCategories.Helmet:
				case ItemCategories.Armor:
				case ItemCategories.Belt:
				case ItemCategories.Gloves:
				case ItemCategories.Boots:

				case ItemCategories.Accessories:
				case ItemCategories.RiftGem:
					return true;

				default:
					return false;
			}
		}

		public static bool IsWeapon(this ItemCategories key)
		{
			switch (key)
			{
				case ItemCategories.Sword:
				case ItemCategories.Knife:
				case ItemCategories.Axe:
				case ItemCategories.Blunt:
				case ItemCategories.Javelin:
					return true;

				default:
					return false;
			}
		}

		public static bool IsArmor(this ItemCategories key)
		{
			switch (key)
			{
				case ItemCategories.Helmet:
				case ItemCategories.Armor:
				case ItemCategories.Belt:
				case ItemCategories.Gloves:
				case ItemCategories.Boots:
					return true;

				default:
					return false;
			}
		}

		public static bool IsAccessories(this ItemCategories key)
		{
			switch (key)
			{
				case ItemCategories.Accessories:
					return true;

				default:
					return false;
			}
		}

		public static bool IsDroppable(this ItemCategories key)
		{
			switch (key)
			{
				case ItemCategories.Sword:      // ��
				case ItemCategories.Knife:      // �ܵ�
				case ItemCategories.Axe:        // ����
				case ItemCategories.Blunt:      // �б�
				case ItemCategories.Javelin:    // â
				case ItemCategories.Talisman:   // Ż������
				case ItemCategories.Focus:      // ���
				case ItemCategories.Helmet: // ����
				case ItemCategories.Armor:  // ����
				case ItemCategories.Belt:   // �㸮��
				case ItemCategories.Gloves: // �尩
				case ItemCategories.Boots:  // ��ȭ
				case ItemCategories.Accessories:    // ��ű�
				case ItemCategories.RiftGem: // �տ� ����
					return true;

				case ItemCategories.Quest:              // ����Ʈ
				case ItemCategories.CraftingMaterial:   // ���� ���
				case ItemCategories.Refill:             // ������
				case ItemCategories.Essence:            // ����
				case ItemCategories.SkillBook:          // ����� ��
				case ItemCategories.Prayers:            // �⵵ (���� ��ȭ+)
					return false;

				default:
					return false;
			}
		}
	}
}