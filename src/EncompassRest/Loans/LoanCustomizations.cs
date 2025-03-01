﻿using System;
using System.ComponentModel;
using System.Linq;
using EncompassRest.Utilities;
using Newtonsoft.Json;

namespace EncompassRest.Loans
{
    partial class Loan
    {
        private LoanFields _fields;
        private ILoanObjectBoundApis _loanApis;

        /// <summary>
        /// The <see cref="IEncompassRestClient"/> associated with this object.
        /// </summary>
        [JsonIgnore]
        public IEncompassRestClient Client { get; internal set; }

        /// <summary>
        /// The Loan Apis for this loan. Loan object must be initialized to use this.
        /// </summary>
        [JsonIgnore]
        public ILoanObjectBoundApis LoanApis => _loanApis ?? throw new InvalidOperationException("Loan object must be initialized to use LoanApis");

        /// <summary>
        /// The loan fields collection.
        /// </summary>
        [JsonIgnore]
        public LoanFields Fields => _fields ?? (_fields = new LoanFields(this));

        [IdPropertyName(nameof(EncompassId))]
        string IIdentifiable.Id { get => EncompassId ?? Id; set { EncompassId = value; Id = value; } }

        private Application _currentApplication;

        /// <summary>
        /// The current application/borrower pair.
        /// </summary>
        [JsonIgnore]
        public Application CurrentApplication
        {
            get
            {
                var currentApplication = _currentApplication;
                if (currentApplication == null)
                {
                    var applicationIndex = CurrentApplicationIndex ?? 0;
                    CurrentApplicationIndex = applicationIndex;
                    currentApplication = Applications.FirstOrDefault(a => a.ApplicationIndex == applicationIndex);
                    if (currentApplication == null)
                    {
                        currentApplication = new Application { ApplicationIndex = applicationIndex };
                        Applications.Add(currentApplication);
                    }
                    _currentApplication = currentApplication;
                }
                return currentApplication;
            }
        }

        /// <summary>
        /// The Loan update constructor.
        /// </summary>
        /// <param name="client">The client to initialize the loan object with.</param>
        /// <param name="loanId">The loan id of the Encompass loan to update.</param>
        public Loan(EncompassRestClient client, string loanId)
            : this((IEncompassRestClient)client, loanId)
        {
        }

        /// <summary>
        /// The Loan update constructor.
        /// </summary>
        /// <param name="client">The client to initialize the loan object with.</param>
        /// <param name="loanId">The loan id of the Encompass loan to update.</param>
        public Loan(IEncompassRestClient client, string loanId)
        {
            Initialize(client, loanId);
        }

        /// <summary>
        /// The Loan creation constructor.
        /// </summary>
        /// <param name="client">The client to associate the object with.</param>
        public Loan(EncompassRestClient client)
            : this((IEncompassRestClient)client)
        {
        }

        /// <summary>
        /// The Loan creation constructor.
        /// </summary>
        /// <param name="client">The client to associate the object with.</param>
        public Loan(IEncompassRestClient client)
        {
            Preconditions.NotNull(client, nameof(client));

            Client = client;
        }

        /// <summary>
        /// The Loan deserialization constructor.
        /// </summary>
        [JsonConstructor]
        [Obsolete("Use EncompassRestClient parameter constructor instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Loan()
        {
        }

        /// <summary>
        /// Initializes the loan object with the specified <paramref name="client"/> and <paramref name="loanId"/>. This allows the use of the <see cref="LoanApis"/> property.
        /// </summary>
        /// <param name="client">The client to initialize the loan object with.</param>
        /// <param name="loanId">The loan id of the Encompass loan.</param>
        public void Initialize(EncompassRestClient client, string loanId) => Initialize((IEncompassRestClient)client, loanId);

        /// <summary>
        /// Initializes the loan object with the specified <paramref name="client"/> and <paramref name="loanId"/>. This allows the use of the <see cref="LoanApis"/> property.
        /// </summary>
        /// <param name="client">The client to initialize the loan object with.</param>
        /// <param name="loanId">The loan id of the Encompass loan.</param>
        public void Initialize(IEncompassRestClient client, string loanId)
        {
            Preconditions.NotNull(client, nameof(client));
            Preconditions.NotNullOrEmpty(loanId, nameof(loanId));

            if (!string.IsNullOrEmpty(EncompassId) && !string.Equals(EncompassId, loanId, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Cannot initialize with different loanId");
            }

            if (!ReferenceEquals(Client, client) || _loanApis == null)
            {
                Client = client;
                EncompassId = loanId;
                _encompassId.Dirty = false;
                _loanApis = client.Loans.GetLoanApis(this);
            }
        }

        internal override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(CurrentApplicationIndex):
                    _currentApplication = null;
                    break;
            }
        }
    }
}