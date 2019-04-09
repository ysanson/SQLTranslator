using System;
using System.Collections.Generic;
using System.Text;

namespace SQLTranslator.Destination
{
    /// <summary>
    /// Interface for the overall generations.
    /// Every possible destination should implement this interface.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// This method creates a CREATE TABLE statement. 
        /// </summary>
        /// <param name="inputBloc">The original CREATE TABLE statement.</param>
        /// <returns>The translated CREATE TABLE statement.</returns>
        List<string> GenerateTable(List<string> inputBloc);

        /// <summary>
        /// This method creates an INSERT INTO statement.
        /// </summary>
        /// <param name="inputLine">The original INSERT INTO statement.</param>
        /// <returns>The translated INSERT INTO statement.</returns>
        string GenerateInsert(string inputLine);


        /// <summary>
        /// This method creates a DROP TABLE statement.
        /// </summary>
        /// <param name="inputLine">The original DROP TABLE statement.</param>
        /// <returns>The translated DROP TABLE statement.</returns>
        string GenerateDrop(string inputLine);
    }
}
