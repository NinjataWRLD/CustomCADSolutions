﻿using CustomCADSolutions.Core.Models;
using static CustomCADSolutions.Core.TestsErrorMessages;

namespace CustomCADSolutions.Tests.ServiceTests.CategoryTests
{
    public class EditAsyncTests : BaseCategoriesTests
    {
        [TestCase(1)]
        [TestCase(4)]
        public async Task Test_EditsDesiredProperties(int id)
        {
            CategoryModel expectedCategory = await service.GetByIdAsync(id);
            expectedCategory.Name = "Edited Category";

            await service.EditAsync(id, expectedCategory);
            CategoryModel actualCategory = await service.GetByIdAsync(id);

            Assert.That(actualCategory.Name, Is.EqualTo(expectedCategory.Name),
                string.Format(DoesNotEditEnough, "Name"));
        }

        [TestCase(1)]
        [TestCase(4)]
        public async Task Test_DoesNotEditUndesiredProperties(int id)
        {
            CategoryModel expectedCategory = await service.GetByIdAsync(id);
            
            expectedCategory.Id = 100;
            CategoryModel actualCategory = await service.GetByIdAsync(id);
            
            Assert.That(actualCategory.Id, Is.Not.EqualTo(expectedCategory.Id),
                    string.Format(EditsTooMuch, "Id"));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(100)]
        public void Test_ThrowsWhenCategoryDoesNotExist(int id)
        {
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await service.EditAsync(id, new());
            }, string.Format(EditsNonExistent, "Category"));
        }
    }
}
