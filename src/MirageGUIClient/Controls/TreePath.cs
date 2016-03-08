using System;
using System.Collections.Generic;
using System.Text;

namespace MirageGUI.Controls
{

    public class TreeEventArgs : System.EventArgs
    {
        private TreePath _path;

        public TreeEventArgs() : this(TreePath.EmptyPath)
        {
        }

        public TreeEventArgs(TreePath path)
        {
            _path = path;
        }

        public MirageGUI.Controls.TreePath Path
        {
            get { return this._path; }
        }
    }

    public class TreeDataEventArgs : TreeEventArgs
    {
        private bool _isRecursive = true;
        private object _data;

        public TreeDataEventArgs(TreePath path, bool isRecursive, object data) : base(path)
        {
            _isRecursive = isRecursive;
            _data = data;
        }

        public bool IsRecursive
        {
            get { return _isRecursive; }
        }

        public object Data
        {
            get { return _data; }
        }
    }

    public class TreePath
    {
        private object[] _path;
        public static TreePath EmptyPath = new TreePath();

        public TreePath()
        {
            _path = new object[0];
        }

        public TreePath(object[] path)
        {
            _path = path;
        }

        public TreePath(TreePath basePath, object nextNode)
        {
            _path = new object[basePath.Count + 1];
            basePath._path.CopyTo(_path, 0);
            _path[_path.Length - 1] = nextNode;
        }

        public TreePath(object item)
        {
            _path = new object[] { item };
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                if (obj is TreePath)
                {
                    TreePath otherPath = (TreePath)obj;
                    if (otherPath.Count == this.Count)
                    {
                        for (int i = 0; i < otherPath._path.Length; i++)
                        {
                            if (!otherPath._path[i].Equals(_path[i]))
                                return false;
                        }
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }
        public TreePath Append(object nextElement)
        {
            return new TreePath(this, nextElement);
        }

        public object[] FullPath
        {
            get { return _path; }
        }

        public TreePath ParentPath
        {
            get
            {
                if (Count > 1)
                {
                    object[] newPath = new object[Count - 1];
                    Array.Copy(_path, 0, newPath, 0, newPath.Length);
                    return new TreePath(newPath);
                }
                else
                {
                    return EmptyPath;
                }
            }
        }

        /// <summary>
        /// Checks to see if the parent path is a parent
        /// of this path.
        /// </summary>
        /// <param name="parent">parent path</param>
        /// <returns>true if this node is a child of the parent</returns>
        public bool IsChildOf(TreePath parent)
        {
            
            int length = parent.Count;
            if (length >= Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    if (!_path.Equals(parent._path[i]))
                        return false;
                }
                return true;
            }
        }

        public object FirstNode
        {
            get
            {
                if (!IsEmpty)
                    return _path[0];
                else
                    return null;
            }
        }

        public object LastNode
        {
            get
            {
                if (!IsEmpty)
                    return _path[_path.Length - 1];
                else
                    return null;
            }
        }

        /// <summary>
        /// The number of nodes in the path
        /// </summary>
        public int Count
        {
            get { return IsEmpty ? 0 : _path.Length; }
        }

        /// <summary>
        /// Returns true if the path is empty
        /// </summary>
        public bool IsEmpty
        {
            get { return _path == null || _path.Length == 0; }
        }

        public override string ToString()
        {
            return ToString('/');
        }

        public string ToString(char PathSeparator)
        {
            if (IsEmpty)
            {
                return "";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _path.Length; i++)
                {
                    if (i > 0)
                        sb.Append(PathSeparator);

                    sb.Append(_path[i].ToString());
                }
                return sb.ToString();
            }
        }
    }
}
