using System.IO;

interface IFileStore
{
	Stream Read(IPathKey key);
	void Write(IPathKey key, Stream stream);
}
interface IPathKey {}

namespace FileStore1
{
	class FileSystemFileStore : IFileStore
	{
		IFileSystemReader reader;
		IFileSystemWriter writer;
		IFileSystemPathGenerator generator;

		public Stream Read(IPathKey key)
		{
			string absolutePath = this.generator.Generate(key);
			return this.reader.Read(absolutePath);
		}
		public void Write(IPathKey key, Stream stream)
		{
			string absolutePath = this.generator.Generate(key);
			this.writer.Write(absolutePath, stream);
		}

		interface IFileSystemReader
		{
			Stream Read (string absolutePath);
		}
		interface IFileSystemWriter
		{
			void Write (string absolutePath, Stream stream);
		}
		interface IFileSystemPathGenerator
		{
			string Generate (IPathKey key);
		}
	}

	class AwsFileStore : IFileStore
	{
		IAwsKeyGenerator generator;
		IAwsReader reader;
		IAwsWriter writer;

		public Stream Read(IPathKey key)
		{
			S3Key s3key = this.generator.Generate(key);
			return this.reader.Read(s3key);
		}
		public void Write(IPathKey key, Stream stream)
		{
			S3Key s3key = this.generator.Generate(key);
			this.writer.Write(s3key, stream);
		}

		interface IAwsReader
		{
			Stream Read (S3Key absolutePath);
		}
		interface IAwsWriter
		{
			void Write (S3Key absolutePath, Stream stream);
		}
		interface IAwsKeyGenerator
		{
			S3Key Generate (IPathKey key);
		}
		class S3Key {}
	}

	class RackspaceCloudFileStore : IFileStore
	{
		IRackspaceContainerNameGenerator cGenerator;
		IRackspaceObjectPathGenerator pathGenerator;
		IRackspaceReader reader;
		IRackspaceWriter writer;
		public Stream Read(IPathKey key)
		{
			string containerName = this.cGenerator.Generate(key);
			string objectPath = this.pathGenerator.Generate(key);
			return this.reader.Read(containerName, objectPath);
		}
		public void Write(IPathKey key, Stream stream)
		{
			string containerName = this.cGenerator.Generate(key);
			string objectPath = this.pathGenerator.Generate(key);
			this.writer.Write(containerName, objectPath, stream);
		}

		interface IRackspaceReader
		{
			Stream Read(string containerName, string objectPath);
		}
		interface IRackspaceWriter
		{
			void Write(string containerName, string objectPath, Stream stream);
		}
		interface IRackspaceContainerNameGenerator
		{
			string Generate(IPathKey key);
		}
		interface IRackspaceObjectPathGenerator
		{
			string Generate(IPathKey key);
		}
	}
}


namespace FileStore2
{
	class FileStore<TPath> : IFileStore
	{
		IFileReader<TPath> reader;
		IFileWriter<TPath> writer;
		IPathGenerator<TPath> generator;

		public Stream Read(IPathKey key)
		{
			TPath path = this.generator.Generate(key);
			return this.reader.Read(path);
		}
		public void Write(IPathKey key, Stream stream)
		{
			TPath path = this.generator.Generate(key);
			this.writer.Write(path, stream);
		}
	}
	interface IFileReader<in TPath>
	{
		Stream Read(TPath path);
	}
	interface IFileWriter<in TPath>
	{
		void Write(TPath path, Stream stream);
	}
	interface IPathGenerator<out TPath>
	{
		TPath Generate(IPathKey key);
	}
}

