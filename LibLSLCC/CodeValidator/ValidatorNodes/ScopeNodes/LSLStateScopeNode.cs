using System;
using System.Collections.Generic;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

namespace LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes
{
    public class LSLStateScopeNode : ILSLStateScopeNode, ILSLSyntaxTreeNode
    {
        private readonly List<LSLEventHandlerNode> _eventHandlers = new List<LSLEventHandlerNode>();


// ReSharper push UnusedParameter.Local
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLStateScopeNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }



        internal LSLStateScopeNode(LSLParser.DefaultStateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }


            StateName = "default";
            DefaultStateContext = context;
            IsDefaultState = true;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLStateScopeNode(LSLParser.DefinedStateContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }


            StateName = context.state_name.Text;
            IsDefinedState = true;
            DefinedStateContext = context;
            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLStateScopeNode(LSLParser.DefaultStateContext context, IEnumerable<LSLEventHandlerNode> eventHandlers)
            : this(context)

        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }


            foreach (var lslEventHandlerNode in eventHandlers)
            {
                AddEventHandler(lslEventHandlerNode);
            }

            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLStateScopeNode(LSLParser.DefinedStateContext context, IEnumerable<LSLEventHandlerNode> eventHandlers)
            : this(context)
        {
            if (eventHandlers == null)
            {
                throw new ArgumentNullException("eventHandlers");
            }


            foreach (var lslEventHandlerNode in eventHandlers)
            {
                AddEventHandler(lslEventHandlerNode);
            }

            SourceCodeRange = new LSLSourceCodeRange(context);
        }



        internal LSLParser.DefinedStateContext DefinedStateContext { get; private set; }
        internal LSLParser.DefaultStateContext DefaultStateContext { get; private set; }

        public IReadOnlyList<LSLEventHandlerNode> EventHandlers
        {
            get { return _eventHandlers; }
        }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }


        public string StateName { get; private set; }

        public bool IsDefinedState { get; private set; }
        public bool IsDefaultState { get; private set; }


        IReadOnlyList<ILSLEventHandlerNode> ILSLStateScopeNode.EventHandlers
        {
            get { return _eventHandlers; }
        }




        #region ILSLTreeNode Members


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }



        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (IsDefaultState)
            {
                return visitor.VisitDefaultState(this);
            }

            return visitor.VisitDefinedState(this);
        }



        public ILSLSyntaxTreeNode Parent { get; set; }


        #endregion




        #region Nested type: Err


        protected enum Err
        {
            Err
        }


        #endregion




        public static
            LSLStateScopeNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLStateScopeNode(sourceRange, Err.Err);
        }



        public void AddEventHandler(LSLEventHandlerNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Parent = this;
            _eventHandlers.Add(node);
        }
    }
}